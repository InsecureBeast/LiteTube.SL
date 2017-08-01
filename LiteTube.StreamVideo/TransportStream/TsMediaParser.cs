using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Buffering;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Pes;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.TransportStream
{
  public sealed class TsMediaParser : IMediaParser, IDisposable
  {
    private static readonly MediaStream[] NoMediaStreams = new MediaStream[0];
    private MediaStream[] _mediaStreams = TsMediaParser.NoMediaStreams;
    private readonly IBufferPool _bufferPool;
    private readonly IPesHandlers _pesHandlers;
    private readonly ITsDecoder _tsDecoder;
    private readonly ITsPesPacketPool _tsPesPacketPool;
    private readonly ITsTimestamp _tsTimemestamp;
    private IBufferingManager _bufferingManager;
    private int? _streamCount;

    public ICollection<IMediaParserMediaStream> MediaStreams
    {
      get
      {
        return (ICollection<IMediaParserMediaStream>) Enumerable.ToArray<MediaStream>((IEnumerable<MediaStream>) this._mediaStreams);
      }
    }

    public TimeSpan StartPosition
    {
      get
      {
        return this._tsTimemestamp.StartPosition;
      }
      set
      {
        this._tsTimemestamp.StartPosition = value;
      }
    }

    public bool EnableProcessing
    {
      get
      {
        return this._tsDecoder.EnableProcessing;
      }
      set
      {
        this._tsDecoder.EnableProcessing = value;
      }
    }

    public event EventHandler ConfigurationComplete;

    public TsMediaParser(ITsDecoder tsDecoder, ITsPesPacketPool tsPesPacketPool, IBufferPool bufferPool, ITsTimestamp tsTimemestamp, IPesHandlers pesHandlers)
    {
      if (null == tsDecoder)
        throw new ArgumentNullException("tsDecoder");
      if (null == tsPesPacketPool)
        throw new ArgumentNullException("tsPesPacketPool");
      if (null == bufferPool)
        throw new ArgumentNullException("bufferPool");
      if (null == tsTimemestamp)
        throw new ArgumentNullException("tsTimemestamp");
      if (null == pesHandlers)
        throw new ArgumentNullException("pesHandlers");
      this._tsPesPacketPool = tsPesPacketPool;
      this._bufferPool = bufferPool;
      this._tsDecoder = tsDecoder;
      this._tsTimemestamp = tsTimemestamp;
      this._pesHandlers = pesHandlers;
    }

    public void Dispose()
    {
      this.DisposeStreams();
    }

    public void Initialize(IBufferingManager bufferingManager, Action<IProgramStreams> programStreamsHandler = null)
    {
      if (null == bufferingManager)
        throw new ArgumentNullException("bufferingManager");
      this._bufferingManager = bufferingManager;
      Action<IProgramStreams> handler = programStreamsHandler ?? new Action<IProgramStreams>(TsMediaParser.DefaultProgramStreamsHandler);
      programStreamsHandler = (Action<IProgramStreams>) (pss =>
      {
        handler(pss);
        this._streamCount = new int?(Enumerable.Count<IProgramStream>((IEnumerable<IProgramStream>) pss.Streams, (Func<IProgramStream, bool>) (s => !s.BlockStream)));
      });
      this._tsDecoder.Initialize(new Func<TsStreamType, uint, IMediaStreamMetadata, TsPacketizedElementaryStream>(this.CreatePacketizedElementaryStream), programStreamsHandler);
    }

    public void InitializeStream(IStreamMetadata streamMetadata)
    {
    }

    public void StartSegment(ISegmentMetadata segmentMetadata)
    {
    }

    public void SetTrackMetadata(ITrackMetadata trackMetadata)
    {
    }

    public void FlushBuffers()
    {
      this._tsDecoder.FlushBuffers();
      foreach (MediaStream mediaStream in this._mediaStreams)
        mediaStream.Flush();
      this._tsTimemestamp.Flush();
    }

    public void ProcessEndOfData()
    {
      this._tsDecoder.ParseEnd();
      this.PushStreams(true);
      this._bufferingManager.ReportEndOfData();
    }

    public void ProcessData(byte[] buffer, int offset, int length)
    {
      this._tsDecoder.Parse(buffer, offset, length);
      if (!this.PushStreams(false))
        return;
      this._bufferingManager.Refresh();
    }

    private bool PushStreams(bool force)
    {
      if (!this._tsTimemestamp.ProcessPackets() && !force)
        return false;
      bool flag = false;
      foreach (MediaStream mediaStream in this._mediaStreams)
      {
        if (mediaStream.PushPackets())
          flag = true;
      }
      return flag;
    }

    private static void DefaultProgramStreamsHandler(IProgramStreams pss)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (IProgramStream programStream in (IEnumerable<IProgramStream>) pss.Streams)
      {
        switch (programStream.StreamType.Contents)
        {
          case TsStreamType.StreamContents.Audio:
            if (flag1)
            {
              programStream.BlockStream = true;
              break;
            }
            flag1 = true;
            break;
          case TsStreamType.StreamContents.Video:
            if (flag2)
            {
              programStream.BlockStream = true;
              break;
            }
            flag2 = true;
            break;
          default:
            programStream.BlockStream = true;
            break;
        }
      }
    }

    private void DisposeStreams()
    {
      MediaStream[] mediaStreamArray = Interlocked.Exchange<MediaStream[]>(ref this._mediaStreams, TsMediaParser.NoMediaStreams);
      if (mediaStreamArray.Length <= 0)
        return;
      foreach (MediaStream mediaStream in mediaStreamArray)
        mediaStream.Dispose();
    }

    private void AddMediaStream(MediaStream mediaStream)
    {
      MediaStream[] comparand = this._mediaStreams;
      MediaStream[] mediaStreamArray1 = new MediaStream[this._mediaStreams.Length + 1];
      while (true)
      {
        if (mediaStreamArray1.Length != comparand.Length)
          mediaStreamArray1 = new MediaStream[this._mediaStreams.Length + 1];
        Array.Copy((Array) comparand, (Array) mediaStreamArray1, comparand.Length);
        mediaStreamArray1[mediaStreamArray1.Length - 1] = mediaStream;
        MediaStream[] mediaStreamArray2 = Interlocked.CompareExchange<MediaStream[]>(ref this._mediaStreams, mediaStreamArray1, comparand);
        if (!object.ReferenceEquals((object) mediaStreamArray2, (object) comparand))
          comparand = mediaStreamArray2;
        else
          break;
      }
      this.CheckConfigurationComplete();
    }

    private void CheckConfigurationComplete()
    {
      ICollection<IMediaParserMediaStream> mediaStreams = this.MediaStreams;
      if (!this._streamCount.HasValue || this._streamCount.Value != mediaStreams.Count || Enumerable.Any<IMediaParserMediaStream>((IEnumerable<IMediaParserMediaStream>) mediaStreams, (Func<IMediaParserMediaStream, bool>) (stream => stream.ConfigurationSource != null && !stream.ConfigurationSource.IsConfigured)))
        return;
      this.FireConfigurationComplete();
    }

    private void FireConfigurationComplete()
    {
      EventHandler cc = this.ConfigurationComplete;
      if (null == cc)
        return;
      this.ConfigurationComplete = (EventHandler) null;
      Task task = TaskEx.Run((Action) (() => cc((object) this, EventArgs.Empty)));
      TaskCollector.Default.Add(task, "TsMediaParser.FireConfigurationComplete()");
    }

    private TsPacketizedElementaryStream CreatePacketizedElementaryStream(TsStreamType streamType, uint pid, IMediaStreamMetadata mediaStreamMetadata)
    {
      IStreamBuffer streamBuffer = this._bufferingManager.CreateStreamBuffer(streamType);
      MediaStream mediaStream = (MediaStream) null;
      PesStreamHandler pesHandler = this._pesHandlers.GetPesHandler(streamType, pid, mediaStreamMetadata, (Action<TsPesPacket>) (packet =>
      {
        if (null != mediaStream)
        {
          mediaStream.EnqueuePacket(packet);
        }
        else
        {
          if (null == packet)
            return;
          this._tsPesPacketPool.FreePesPacket(packet);
        }
      }));
      TsPacketizedElementaryStream elementaryStream = new TsPacketizedElementaryStream(this._bufferPool, this._tsPesPacketPool, new Action<TsPesPacket>(pesHandler.PacketHandler), streamType, pid);
      IConfigurationSource configurator = pesHandler.Configurator;
      if (null != configurator)
      {
        EventHandler configuratorOnConfigurationComplete = (EventHandler) null;
        configuratorOnConfigurationComplete = (EventHandler) ((o, e) =>
        {
          configurator.ConfigurationComplete -= configuratorOnConfigurationComplete;
          this.CheckConfigurationComplete();
        });
        configurator.ConfigurationComplete += configuratorOnConfigurationComplete;
      }
      mediaStream = new MediaStream(configurator, streamBuffer, new Action<TsPesPacket>(this._tsPesPacketPool.FreePesPacket));
      this.AddMediaStream(mediaStream);
      this._tsTimemestamp.RegisterMediaStream(mediaStream, new Func<TsPesPacket, TimeSpan?>(pesHandler.GetDuration));
      if (null == configurator)
        this.CheckConfigurationComplete();
      return elementaryStream;
    }
  }
}
