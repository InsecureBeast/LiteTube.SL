using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SM.Media.Core.Buffering;
using SM.Media.Core.Metadata;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.MediaParser
{
  public abstract class MediaParserBase<TConfigurator> : IMediaParser, IDisposable where TConfigurator : IConfigurationSource
  {
    private readonly TConfigurator _configurator;
    private readonly TsStreamType _streamType;
    private readonly ITsPesPacketPool _tsPesPacketPool;
    private IBufferingManager _bufferingManager;
    private int _isDisposed;
    private MediaStream _mediaStream;
    private ICollection<IMediaParserMediaStream> _mediaStreams;
    private IStreamBuffer _streamBuffer;

    protected TConfigurator Configurator
    {
      get
      {
        return this._configurator;
      }
    }

    public ICollection<IMediaParserMediaStream> MediaStreams
    {
      get
      {
        return this._mediaStreams;
      }
    }

    public bool EnableProcessing { get; set; }

    public virtual TimeSpan StartPosition { get; set; }

    public event EventHandler ConfigurationComplete;

    protected MediaParserBase(TsStreamType streamType, TConfigurator configurator, ITsPesPacketPool tsPesPacketPool)
    {
      if (null == streamType)
        throw new ArgumentNullException("streamType");
      if (object.ReferenceEquals((object) default (TConfigurator), (object) configurator))
        throw new ArgumentNullException("configurator");
      if (null == tsPesPacketPool)
        throw new ArgumentNullException("tsPesPacketPool");
      this._streamType = streamType;
      this._configurator = configurator;
      this._tsPesPacketPool = tsPesPacketPool;
      this._configurator.ConfigurationComplete += new EventHandler(this.OnConfigurationComplete);
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual void ProcessEndOfData()
    {
      this.FlushBuffers();
      this.SubmitPacket((TsPesPacket) null);
      this._mediaStream.PushPackets();
      this._bufferingManager.ReportEndOfData();
    }

    public abstract void ProcessData(byte[] buffer, int offset, int length);

    public virtual void FlushBuffers()
    {
      this._mediaStream.Flush();
    }

    public void Initialize(IBufferingManager bufferingManager, Action<IProgramStreams> programStreamsHandler = null)
    {
      if (null == bufferingManager)
        throw new ArgumentNullException("bufferingManager");
      this._bufferingManager = bufferingManager;
      this._streamBuffer = bufferingManager.CreateStreamBuffer(this._streamType);
      this._mediaStream = new MediaStream((IConfigurationSource) this._configurator, this._streamBuffer, new Action<TsPesPacket>(this._tsPesPacketPool.FreePesPacket));
      this._mediaStreams = (ICollection<IMediaParserMediaStream>) new MediaStream[1]
      {
        this._mediaStream
      };
    }

    public abstract void InitializeStream(IStreamMetadata streamMetadata);

    public abstract void StartSegment(ISegmentMetadata segmentMetadata);

    public abstract void SetTrackMetadata(ITrackMetadata trackMetadata);

    protected virtual bool PushStreams()
    {
      if (!this._mediaStream.PushPackets())
        return false;
      this._bufferingManager.Refresh();
      return true;
    }

    private void OnConfigurationComplete(object sender, EventArgs eventArgs)
    {
      this._configurator.ConfigurationComplete -= new EventHandler(this.OnConfigurationComplete);
      EventHandler eventHandler = this.ConfigurationComplete;
      if (null == eventHandler)
        return;
      eventHandler((object) this, EventArgs.Empty);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (!object.Equals((object) default (TConfigurator), (object) this._configurator))
        this._configurator.ConfigurationComplete -= new EventHandler(this.OnConfigurationComplete);
      if (null != this.ConfigurationComplete)
      {
        Debug.WriteLine("MediaParserBase<>.Dispose(bool) ConfigurationComplete event is still subscribed");
        this.ConfigurationComplete = (EventHandler) null;
      }
      using (this._streamBuffer)
        ;
      using (this._mediaStream)
        ;
      this._mediaStreams = (ICollection<IMediaParserMediaStream>) null;
      this._mediaStream = (MediaStream) null;
      this._bufferingManager = (IBufferingManager) null;
      this._streamBuffer = (IStreamBuffer) null;
    }

    public void SubmitPacket(TsPesPacket packet)
    {
      this._mediaStream.EnqueuePacket(packet);
    }
  }
}
