using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SM.Media.Core;
using SM.Media.Core.Buffering;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media
{
  public sealed class StreamBuffer : IBufferingQueue, IStreamBuffer, IStreamSource, IDisposable
  {
    private readonly Queue<TsPesPacket> _packets = new Queue<TsPesPacket>();
    private readonly object _packetsLock = new object();
    private readonly int _streamBufferId = Interlocked.Increment(ref StreamBuffer._streamBufferCounter);
    private readonly TsStreamType _streamType;
    private Action<TsPesPacket> _freePesPacket;
    private IBufferingManager _bufferingManager;
    private int _isDisposed;
    private bool _isDone;
    private TimeSpan? _oldest;
    private TimeSpan? _newest;
    private int _size;
    private readonly bool _isMedia;
    private bool _eofPacketRead;
    private static int _streamBufferCounter;

    public TsStreamType StreamType
    {
      get
      {
        return this._streamType;
      }
    }

    public bool IsEof
    {
      get
      {
        return this._isDone;
      }
    }

    public TimeSpan? PresentationTimestamp
    {
      get
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._packetsLock, ref lockTaken);
          if (!this._isDone && this._bufferingManager.IsBuffering)
            return new TimeSpan?();
          if (this._packets.Count < 1)
            return new TimeSpan?();
          TsPesPacket tsPesPacket = this._packets.Peek();
          if (null == tsPesPacket)
            return new TimeSpan?();
          return new TimeSpan?(tsPesPacket.PresentationTimestamp);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
    }

    public bool HasSample
    {
      get
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._packetsLock, ref lockTaken);
          return this._packets.Count > 0;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
    }

    public float? BufferingProgress
    {
      get
      {
        return this._bufferingManager.BufferingProgress;
      }
    }

    public StreamBuffer(TsStreamType streamType, Action<TsPesPacket> freePesPacket, IBufferingManager bufferingManager)
    {
      if (null == streamType)
        throw new ArgumentNullException("streamType");
      if (null == freePesPacket)
        throw new ArgumentNullException("freePesPacket");
      if (null == bufferingManager)
        throw new ArgumentNullException("bufferingManager");
      this._streamType = streamType;
      this._freePesPacket = freePesPacket;
      this._bufferingManager = bufferingManager;
      this._isMedia = TsStreamType.StreamContents.Audio == this._streamType.Contents || TsStreamType.StreamContents.Video == this._streamType.Contents;
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._packetsLock, ref lockTaken);
        while (this._packets.Count > 0)
        {
          TsPesPacket tsPesPacket = this._packets.Dequeue();
          if (null != tsPesPacket)
            this._freePesPacket(tsPesPacket);
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      this._freePesPacket = (Action<TsPesPacket>) null;
      this._bufferingManager = (IBufferingManager) null;
    }

    private void ThrowIfDisposed()
    {
      if (0 != this._isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    public TsPesPacket GetNextSample()
    {
      this.ThrowIfDisposed();
      TsPesPacket tsPesPacket1 = (TsPesPacket) null;
      try
      {
        bool flag = false;
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._packetsLock, ref lockTaken);
          if (!this._isDone && this._bufferingManager.IsBuffering)
            return (TsPesPacket) null;
          if (this._packets.Count > 0)
          {
            tsPesPacket1 = this._packets.Dequeue();
            if (null != tsPesPacket1)
            {
              this._oldest = new TimeSpan?(tsPesPacket1.PresentationTimestamp);
              if (!this._newest.HasValue)
                this._newest = this._oldest;
              this._size -= tsPesPacket1.Length;
            }
            else
              this._eofPacketRead = true;
          }
          else
            flag = true;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
        if (flag)
          this._bufferingManager.ReportExhaustion();
        else
          this._bufferingManager.Refresh();
        if (null == tsPesPacket1)
          return (TsPesPacket) null;
        TsPesPacket tsPesPacket2 = tsPesPacket1;
        tsPesPacket1 = (TsPesPacket) null;
        return tsPesPacket2;
      }
      catch (Exception ex)
      {
        Debug.WriteLine("GetNextSample: " + ex.Message);
      }
      finally
      {
        if (null != tsPesPacket1)
          this._freePesPacket(tsPesPacket1);
        this.ThrowIfDisposed();
      }
      return (TsPesPacket) null;
    }

    public void FreeSample(TsPesPacket packet)
    {
      this._freePesPacket(packet);
    }

    public bool DiscardPacketsBefore(TimeSpan value)
    {
      bool flag1 = false;
      bool lockTaken = false;
      object obj = null;
      bool flag2;
      try
      {
        Monitor.Enter(obj = this._packetsLock, ref lockTaken);
        while (this._packets.Count > 0)
        {
          TsPesPacket packet = this._packets.Peek();
          if (!(packet.PresentationTimestamp >= value))
          {
            this._packets.Dequeue();
            this.FreeSample(packet);
            flag1 = true;
          }
          else
            break;
        }
        flag2 = !this._bufferingManager.IsBuffering;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (flag2)
        this._bufferingManager.ReportExhaustion();
      else
        this._bufferingManager.Refresh();
      return flag1;
    }

    void IBufferingQueue.Flush()
    {
      TsPesPacket[] tsPesPacketArray = (TsPesPacket[]) null;
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._packetsLock, ref lockTaken);
        if (this._packets.Count > 0)
        {
          tsPesPacketArray = this._packets.ToArray();
          this._packets.Clear();
        }
        this._size = 0;
        this._newest = new TimeSpan?();
        this._oldest = new TimeSpan?();
        if (!this._eofPacketRead)
          this._isDone = false;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (null == tsPesPacketArray)
        return;
      foreach (TsPesPacket tsPesPacket in tsPesPacketArray)
      {
        if (null != tsPesPacket)
          this._freePesPacket(tsPesPacket);
      }
    }

    public void UpdateBufferStatus(BufferStatus bufferStatus)
    {
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._packetsLock, ref lockTaken);
        bufferStatus.PacketCount = this._packets.Count;
        bufferStatus.Size = this._size;
        bufferStatus.Newest = this._newest;
        bufferStatus.Oldest = this._oldest;
        bufferStatus.IsValid = this._packets.Count > 0 && this._newest.HasValue && this._oldest.HasValue;
        bufferStatus.IsDone = this._isDone;
        bufferStatus.IsMedia = this._isMedia;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    public bool TryEnqueue(ICollection<TsPesPacket> packets)
    {
      this.ThrowIfDisposed();
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._packetsLock, ref lockTaken);
        foreach (TsPesPacket tsPesPacket in (IEnumerable<TsPesPacket>) packets)
        {
          if (tsPesPacket != null && tsPesPacket.Length < 1)
          {
            Debug.WriteLine("StreamBuffer.TryEnqueue() discarding short buffer: size " + (object) tsPesPacket.Length);
            this._freePesPacket(tsPesPacket);
          }
          else if (this._isDone)
          {
            if (null != tsPesPacket)
              this._freePesPacket(tsPesPacket);
          }
          else
          {
            this._packets.Enqueue(tsPesPacket);
            if (null != tsPesPacket)
            {
              this._newest = new TimeSpan?(tsPesPacket.PresentationTimestamp);
              if (!this._oldest.HasValue)
                this._oldest = this._newest;
              this._size += tsPesPacket.Length;
            }
            else
              this._isDone = true;
          }
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      this.ThrowIfDisposed();
      return true;
    }

    public override string ToString()
    {
      BufferStatus bufferStatus = new BufferStatus();
      this.UpdateBufferStatus(bufferStatus);
      return string.Format("{0} count {1} size {2} newest {3} oldest {4}", (object) this.StreamType.Contents, (object) bufferStatus.PacketCount, (object) bufferStatus.Size, (object) bufferStatus.Newest, (object) bufferStatus.Oldest);
    }
  }
}
