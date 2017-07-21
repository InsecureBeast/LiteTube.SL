using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Buffering
{
  public sealed class BufferingManager : IBufferingManager, IDisposable
  {
    private static readonly TimeSpan SeekEndTolerance = TimeSpan.FromMilliseconds(256.0);
    private static readonly TimeSpan SeekBeginTolerance = TimeSpan.FromSeconds(6.0);
    private static readonly TimeSpan BufferStatusUpdatePeriod = TimeSpan.FromMilliseconds(250.0);
    private readonly CancellationTokenSource _disposeCancellationTokenSource = new CancellationTokenSource();
    private readonly object _lock = new object();
    private readonly List<IBufferingQueue> _queues = new List<IBufferingQueue>();
    private readonly List<BufferStatus> _statuses = new List<BufferStatus>();
    private DateTime _bufferStatusTimeUtc = DateTime.MinValue;
    private bool _isBuffering = true;
    private bool _isStarting = true;
    private readonly IBufferingPolicy _bufferingPolicy;
    private readonly ITsPesPacketPool _packetPool;
    private readonly SignalTask _refreshTask;
    private bool _blockReads;
    private float _bufferingProgress;
    private int _isDisposed;
    private bool _isEof;
    private IQueueThrottling _queueThrottling;
    private SignalTask _reportingTask;
    private int _totalBufferedStart;

    public bool IsBuffering
    {
      get
      {
        return this._isBuffering;
      }
    }

    public float? BufferingProgress
    {
      get
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          if (!this.IsBuffering)
            return new float?();
          Debug.Assert((double) this._bufferingProgress >= 0.0 && (double) this._bufferingProgress <= 1.0, "BufferingProgress out of range: " + (object) this._bufferingProgress);
          return new float?(this._bufferingProgress);
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
    }

    public BufferingManager(IBufferingPolicy bufferingPolicy, ITsPesPacketPool packetPool)
    {
      if (null == bufferingPolicy)
        throw new ArgumentNullException("bufferingPolicy");
      if (null == packetPool)
        throw new ArgumentNullException("packetPool");
      this._bufferingPolicy = bufferingPolicy;
      this._packetPool = packetPool;
      this._refreshTask = new SignalTask((Func<Task>) (() =>
      {
        this.RefreshHandler();
        return TplTaskExtensions.CompletedTask;
      }), this._disposeCancellationTokenSource.Token);
    }

    public void Initialize(IQueueThrottling queueThrottling, Action reportBufferingChange)
    {
      Debug.WriteLine("BufferingManager.Initialize()");
      if (null == queueThrottling)
        throw new ArgumentNullException("queueThrottling");
      if (reportBufferingChange == null)
        throw new ArgumentNullException("reportBufferingChange");
      this.ThrowIfDisposed();
      if (null != Interlocked.CompareExchange<IQueueThrottling>(ref this._queueThrottling, queueThrottling, (IQueueThrottling) null))
        throw new InvalidOperationException("The buffering manager is in use");
      this.HandleStateChange();
      this._reportingTask = new SignalTask((Func<Task>) (() =>
      {
        reportBufferingChange();
        return TplTaskExtensions.CompletedTask;
      }), this._disposeCancellationTokenSource.Token);
    }

    public void Shutdown(IQueueThrottling queueThrottling)
    {
      Debug.WriteLine("BufferingManager.Shutdown()");
      if (null == queueThrottling)
        throw new ArgumentNullException("queueThrottling");
      this.ThrowIfDisposed();
      if (!object.ReferenceEquals((object) Interlocked.CompareExchange<IQueueThrottling>(ref this._queueThrottling, (IQueueThrottling) null, queueThrottling), (object) queueThrottling))
        throw new InvalidOperationException("Shutting down the wrong queueThrottling instance");
      this.RefreshHandler();
    }

    public IStreamBuffer CreateStreamBuffer(TsStreamType streamType)
    {
      this.ThrowIfDisposed();
      StreamBuffer streamBuffer = new StreamBuffer(streamType, new Action<TsPesPacket>(this._packetPool.FreePesPacket), (IBufferingManager) this);
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this._queues.Add((IBufferingQueue) streamBuffer);
        this.ResizeStatuses();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      return (IStreamBuffer) streamBuffer;
    }

    public void Flush()
    {
      Debug.WriteLine("BufferingManager.Flush()");
      this.ThrowIfDisposed();
      bool lockTaken = false;
      object obj = null;
      IBufferingQueue[] bufferingQueueArray;
      bool flag;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        bufferingQueueArray = this._queues.ToArray();
        this._isStarting = true;
        this._isBuffering = true;
        this._isEof = false;
        flag = bufferingQueueArray.Length > 0;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      foreach (IBufferingQueue bufferingQueue in bufferingQueueArray)
        bufferingQueue.Flush();
      this.ReportBuffering(0.0f);
      if (!flag)
        return;
      this._refreshTask.Fire();
    }

    public bool IsSeekAlreadyBuffered(TimeSpan position)
    {
      Debug.WriteLine("BufferingManager.IsSeekAlreadyBuffered({0})", (object) position);
      this.ThrowIfDisposed();
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this.UnlockedUpdateQueueStatus();
        foreach (BufferStatus bufferStatus in this._statuses)
        {
          TimeSpan timeSpan1 = position;
          TimeSpan? nullable = bufferStatus.Oldest;
          TimeSpan timeSpan2 = BufferingManager.SeekBeginTolerance;
          nullable = nullable.HasValue ? new TimeSpan?(nullable.GetValueOrDefault() - timeSpan2) : new TimeSpan?();
          if ((nullable.HasValue ? (timeSpan1 < nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            return false;
          TimeSpan timeSpan3 = position;
          nullable = bufferStatus.Newest;
          TimeSpan timeSpan4 = BufferingManager.SeekEndTolerance;
          nullable = nullable.HasValue ? new TimeSpan?(nullable.GetValueOrDefault() + timeSpan4) : new TimeSpan?();
          if ((nullable.HasValue ? (timeSpan3 > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            return false;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      return true;
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this._disposeCancellationTokenSource.Cancel();
      using (this._refreshTask)
        ;
      using (this._reportingTask)
        ;
      this._reportingTask = (SignalTask) null;
      this._disposeCancellationTokenSource.Dispose();
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this._queues.Clear();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (null == Interlocked.Exchange<IQueueThrottling>(ref this._queueThrottling, (IQueueThrottling) null))
        return;
      Debug.WriteLine("**** BufferingManager.Dispose() _queueThrottling was not null");
    }

    public void Refresh()
    {
      this.ThrowIfDisposed();
      this._refreshTask.Fire();
    }

    public void ReportExhaustion()
    {
      Debug.WriteLine("BufferingManager.ReportExhaustion()");
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (this._isEof || this._isBuffering)
          return;
        this.UnlockedRefreshHandler();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    public void ReportEndOfData()
    {
      Debug.WriteLine("BufferingManager.ReportEndOfData()");
      bool lockTaken = false;
      object obj = null;
      bool flag;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        flag = this._isEof;
        this._isEof = true;
        this.UnlockedUpdateQueueStatus();
        this.UnlockedReport();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      this._refreshTask.Fire();
      if (flag || null == this._reportingTask)
        return;
      this._reportingTask.Fire();
    }

    private void ResizeStatuses()
    {
      while (this._statuses.Count > this._queues.Count)
        this._statuses.RemoveAt(this._statuses.Count - 1);
      while (this._statuses.Count < this._queues.Count)
        this._statuses.Add(new BufferStatus());
    }

    private void RefreshHandler()
    {
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this.UnlockedRefreshHandler();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    private void UnlockedRefreshHandler()
    {
      this.UnlockedUpdateQueueStatus();
      this.UnlockedReport();
    }

    private void UnlockedUpdateQueueStatus()
    {
      for (int index = 0; index < this._queues.Count; ++index)
        this._queues[index].UpdateBufferStatus(this._statuses[index]);
    }

    private void UnlockedReport()
    {
      bool flag = this.UpdateState();
      if (flag == this._blockReads)
        return;
      this._blockReads = flag;
      this.HandleStateChange();
    }

    private void UnlockedStartBuffering()
    {
      bool flag = this._isBuffering;
      this._isBuffering = true;
      if (!flag)
        return;
      this.ReportBuffering(0.0f);
    }

    private bool UpdateState()
    {
      if (this._statuses.Count <= 0)
        return false;
      TimeSpan timeSpan1 = TimeSpan.MinValue;
      TimeSpan timeSpan2 = TimeSpan.MaxValue;
      TimeSpan timeSpan3 = TimeSpan.MaxValue;
      int num1 = int.MaxValue;
      int num2 = int.MinValue;
      int num3 = 0;
      bool flag1 = false;
      bool flag2 = this._isEof;
      bool isExhausted = false;
      bool isAllExhausted = true;
      foreach (BufferStatus bufferStatus in this._statuses)
      {
        if (!bufferStatus.IsDone)
          flag2 = false;
        if (bufferStatus.IsMedia)
        {
          if (0 == bufferStatus.PacketCount)
            isExhausted = true;
          else
            isAllExhausted = false;
          if (!bufferStatus.IsValid)
          {
            if (timeSpan3 > TimeSpan.Zero)
              timeSpan3 = TimeSpan.Zero;
          }
          else
          {
            num3 += bufferStatus.Size;
            flag1 = true;
            int packetCount = bufferStatus.PacketCount;
            if (packetCount < num1)
              num1 = packetCount;
            if (packetCount > num2)
              num2 = packetCount;
            TimeSpan? newest = bufferStatus.Newest;
            TimeSpan? nullable1 = newest;
            TimeSpan timeSpan4 = timeSpan1;
            if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() > timeSpan4 ? 1 : 0) : 0) != 0)
              timeSpan1 = newest.Value;
            TimeSpan? oldest = bufferStatus.Oldest;
            nullable1 = oldest;
            TimeSpan timeSpan5 = timeSpan2;
            if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() < timeSpan5 ? 1 : 0) : 0) != 0)
              timeSpan2 = oldest.Value;
            nullable1 = newest;
            TimeSpan? nullable2 = oldest;
            nullable1 = nullable1.HasValue & nullable2.HasValue ? new TimeSpan?(nullable1.GetValueOrDefault() - nullable2.GetValueOrDefault()) : new TimeSpan?();
            TimeSpan timeSpan6 = nullable1 ?? TimeSpan.Zero;
            if (timeSpan6 < TimeSpan.Zero)
              timeSpan6 = TimeSpan.Zero;
            if (timeSpan6 < timeSpan3)
              timeSpan3 = timeSpan6;
          }
        }
      }
      if (flag2)
        this._isEof = true;
      TimeSpan timeSpan7 = flag1 ? timeSpan3 : TimeSpan.MaxValue;
      Debug.Assert((timeSpan7 == TimeSpan.MaxValue ? 1 : (timeSpan7 + TimeSpan.FromMilliseconds(500.0) >= TimeSpan.Zero ? 1 : 0)) != 0, string.Format("Invalid timestamp difference: {0} (newest {1} oldest {2} low count {3} high count {4} valid data {5})", (object) timeSpan7, (object) timeSpan1, (object) timeSpan2, (object) num1, (object) num2, (object) (bool) (flag1)));
      if (timeSpan7 <= TimeSpan.Zero)
      {
        timeSpan7 = TimeSpan.Zero;
        flag1 = false;
      }
      if (this._isBuffering)
      {
        if (flag2)
        {
          this._isBuffering = false;
          Debug.WriteLine("BufferingManager.UpdateState done buffering (eof): duration {0} size {1} starting {2} memory {3:F} MiB", flag1 ? (object) timeSpan7.ToString() : (object) "none", flag1 ? (object) num3.ToString() : (object) "none", (object) (bool) (this._isStarting), (object) ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
          this.DumpQueues();
          this.ReportBuffering(1f);
        }
        else if (flag1)
          this.UpdateBuffering(timeSpan7, num3);
        if (!this._isBuffering)
          return false;
      }
      else if (!flag2 && isExhausted)
      {
        Debug.WriteLine("BufferingManager.UpdateState start buffering: duration {0} size {1} starting {2} memory {3:F} MiB", (object) timeSpan7, (object) num3, (object) (bool) (this._isStarting), (object) ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
        this.DumpQueues();
        this._totalBufferedStart = num3;
        this.UnlockedStartBuffering();
        return false;
      }
      if (!flag1)
        return false;
      bool flag3 = this._bufferingPolicy.ShouldBlockReads(this._blockReads, timeSpan7, num3, isExhausted, isAllExhausted);
      if (flag3 != this._blockReads)
      {
        Debug.WriteLine("BufferingManager.UpdateState read blocking -> {0} duration {1} size {2} memory {3:F} MiB", (object) (bool) (flag3), flag1 ? (object) timeSpan7.ToString() : (object) "none", flag1 ? (object) num3.ToString() : (object) "none", (object) ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
        this.DumpQueues();
      }
      return flag3;
    }

    [Conditional("DEBUG")]
    private void DumpQueues()
    {
      foreach (object obj in this._queues)
        Debug.WriteLine("  " + obj);
    }

    private void UpdateBuffering(TimeSpan timestampDifference, int bufferSize)
    {
      if (this._bufferingPolicy.IsDoneBuffering(timestampDifference, bufferSize, this._totalBufferedStart, this._isStarting))
      {
        this._isBuffering = false;
        Debug.WriteLine("BufferingManager.UpdateBuffering done buffering: duration {0} size {1} starting {2} memory {3:F} MiB", (object) timestampDifference, (object) bufferSize, (object) (bool) (this._isStarting), (object) ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
        this.DumpQueues();
        this._isStarting = false;
        this.ReportBuffering(1f);
      }
      else
      {
        DateTime utcNow = DateTime.UtcNow;
        if (utcNow - this._bufferStatusTimeUtc >= BufferingManager.BufferStatusUpdatePeriod)
        {
          this._bufferStatusTimeUtc = utcNow;
          float progress = this._bufferingPolicy.GetProgress(timestampDifference, bufferSize, this._totalBufferedStart, this._isStarting);
          Debug.WriteLine("BufferingManager.UpdateBuffering: {0:F2}% duration {1} size {2} starting {3} memory {4:F} MiB", (object) (float) ((double) progress * 100.0), (object) timestampDifference, (object) bufferSize, (object) (bool) (this._isStarting), (object) ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
          this.ReportBuffering(progress);
        }
      }
    }

    private void ReportBuffering(float bufferingProgress)
    {
      this._bufferingProgress = bufferingProgress;
      if (null == this._reportingTask)
        return;
      this._reportingTask.Fire();
    }

    private void HandleStateChange()
    {
      IQueueThrottling queueThrottling = Volatile.Read<IQueueThrottling>(ref this._queueThrottling);
      if (null == queueThrottling)
        return;
      if (this._blockReads)
        queueThrottling.Pause();
      else
        queueThrottling.Resume();
    }

    private void ThrowIfDisposed()
    {
      if (0 != this._isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }
  }
}
