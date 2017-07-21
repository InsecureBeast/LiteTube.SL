// Decompiled with JetBrains decompiler
// Type: SM.Media.Audio.AudioParserBase
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.TransportStream.TsParser;
using SM.Media.TransportStream.TsParser.Utility;
using System;
using System.Diagnostics;
using System.Threading;

namespace SM.Media.Audio
{
  public abstract class AudioParserBase : IAudioParser, IDisposable
  {
    protected readonly IAudioFrameHeader _frameHeader;
    protected int _badBytes;
    protected Action<IAudioFrameHeader> _configurationHandler;
    protected bool _hasSeenValidFrames;
    protected int _index;
    protected bool _isConfigured;
    private int _isDisposed;
    protected TsPesPacket _packet;
    protected ITsPesPacketPool _pesPacketPool;
    private TimeSpan? _position;
    protected int _startIndex;
    protected Action<TsPesPacket> _submitPacket;

    public TimeSpan StartPosition { get; set; }

    public TimeSpan? Position
    {
      get
      {
        return this._position;
      }
      set
      {
        this._position = value;
      }
    }

    protected AudioParserBase(IAudioFrameHeader frameHeader, ITsPesPacketPool pesPacketPool, Action<IAudioFrameHeader> configurationHandler, Action<TsPesPacket> submitPacket)
    {
      if (frameHeader == null)
        throw new ArgumentNullException("frameHeader");
      if (pesPacketPool == null)
        throw new ArgumentNullException("pesPacketPool");
      if (configurationHandler == null)
        throw new ArgumentNullException("configurationHandler");
      if (submitPacket == null)
        throw new ArgumentNullException("submitPacket");
      this._frameHeader = frameHeader;
      this._pesPacketPool = pesPacketPool;
      this._configurationHandler = configurationHandler;
      this._submitPacket = submitPacket;
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual void FlushBuffers()
    {
      this.FreeBuffer();
      this._hasSeenValidFrames = false;
      this._badBytes = 0;
    }

    public abstract void ProcessData(byte[] buffer, int offset, int length);

    protected virtual void SubmitFrame()
    {
      Debug.Assert(this._index >= this._startIndex, "_index less than _startIndex");
      int length = this._index - this._startIndex;
      if (length > 0)
      {
        TsPesPacket tsPesPacket;
        if (this._index + 128 >= this._packet.Buffer.Length)
        {
          tsPesPacket = this._packet;
          this._packet = (TsPesPacket) null;
          tsPesPacket.Length = length;
          tsPesPacket.Index = this._startIndex;
        }
        else
          tsPesPacket = this._pesPacketPool.CopyPesPacket(this._packet, this._startIndex, length);
        if (!this.Position.HasValue)
          this.Position = new TimeSpan?(this.StartPosition);
        tsPesPacket.PresentationTimestamp = this.Position.Value;
        tsPesPacket.Duration = new TimeSpan?(this._frameHeader.Duration);
        AudioParserBase audioParserBase = this;
        // ISSUE: explicit non-virtual call
        TimeSpan? position = __nonvirtual (audioParserBase.Position);
        TimeSpan duration = this._frameHeader.Duration;
        TimeSpan? nullable = position.HasValue ? new TimeSpan?(position.GetValueOrDefault() + duration) : new TimeSpan?();
        // ISSUE: explicit non-virtual call
        __nonvirtual (audioParserBase.Position) = nullable;
        this._submitPacket(tsPesPacket);
        this._hasSeenValidFrames = true;
        this._badBytes = 0;
      }
      this._startIndex = this._index;
      this.EnsureBufferSpace(128);
    }

    protected void EnsureBufferSpace(int length)
    {
      if (null == this._packet)
      {
        this._index = 0;
        this._startIndex = 0;
        this._packet = (TsPesPacket) null;
        this._packet = this.CreatePacket(length);
      }
      else
      {
        if (this._index + length <= this._packet.Buffer.Length)
          return;
        TsPesPacket packet = this.CreatePacket(length);
        if (this._index > this._startIndex)
        {
          this._index -= this._startIndex;
          Array.Copy((Array) this._packet.Buffer, this._startIndex, (Array) packet.Buffer, 0, this._index);
        }
        else
          this._index = 0;
        this._startIndex = 0;
        this._packet.Length = 0;
        this._pesPacketPool.FreePesPacket(this._packet);
        this._packet = packet;
      }
    }

    private TsPesPacket CreatePacket(int length)
    {
      TsPesPacket tsPesPacket = this._pesPacketPool.AllocatePesPacket(length);
      tsPesPacket.Length = tsPesPacket.Buffer.Length;
      return tsPesPacket;
    }

    private void FreeBuffer()
    {
      if (null != this._packet)
      {
        TsPesPacket packet = this._packet;
        this._packet = (TsPesPacket) null;
        packet.Length = 0;
        this._pesPacketPool.FreePesPacket(packet);
      }
      this._startIndex = 0;
      this._index = 0;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.FreeBuffer();
      this._pesPacketPool = (ITsPesPacketPool) null;
      this._configurationHandler = (Action<IAudioFrameHeader>) null;
      this._submitPacket = (Action<TsPesPacket>) null;
    }
  }
}
