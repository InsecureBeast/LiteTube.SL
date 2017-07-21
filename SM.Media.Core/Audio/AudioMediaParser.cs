using System;
using System.Diagnostics;
using SM.Media.Core.Audio.Shoutcast;
using SM.Media.Core.MediaParser;
using SM.Media.Core.Metadata;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.Audio
{
  public abstract class AudioMediaParser<TParser, TConfigurator> : MediaParserBase<TConfigurator> where TParser : class, IAudioParser where TConfigurator : IConfigurationSource
  {
    private readonly IMetadataSink _metadataSink;
    private readonly IShoutcastMetadataFilterFactory _shoutcastMetadataFilterFactory;
    protected TParser Parser;
    private IAudioParser _audioParser;

    public override TimeSpan StartPosition
    {
      get
      {
        return this.Parser.StartPosition;
      }
      set
      {
        this.Parser.StartPosition = value;
      }
    }

    protected AudioMediaParser(TsStreamType streamType, TConfigurator configurator, ITsPesPacketPool pesPacketPool, IShoutcastMetadataFilterFactory shoutcastMetadataFilterFactory, IMetadataSink metadataSink)
      : base(streamType, configurator, pesPacketPool)
    {
      if (null == shoutcastMetadataFilterFactory)
        throw new ArgumentNullException("shoutcastMetadataFilterFactory");
      if (null == metadataSink)
        throw new ArgumentNullException("metadataSink");
      this._shoutcastMetadataFilterFactory = shoutcastMetadataFilterFactory;
      this._metadataSink = metadataSink;
    }

    public override void InitializeStream(IStreamMetadata streamMetadata)
    {
      this._metadataSink.ReportStreamMetadata(this.Parser.Position ?? TimeSpan.Zero, streamMetadata);
    }

    public override void StartSegment(ISegmentMetadata segmentMetadata)
    {
      this._audioParser = (IAudioParser) this.Parser;
      IShoutcastSegmentMetadata shoutcastSegmentMetadata = segmentMetadata as IShoutcastSegmentMetadata;
      if (null != shoutcastSegmentMetadata)
      {
        int? icyMetaInt = shoutcastSegmentMetadata.IcyMetaInt;
        int num;
        if (icyMetaInt.HasValue)
        {
          int? nullable = icyMetaInt;
          num = (nullable.GetValueOrDefault() <= 0 ? 0 : (nullable.HasValue ? 1 : 0)) == 0 ? 1 : 0;
        }
        else
          num = 1;
        if (num == 0)
          this._audioParser = this._shoutcastMetadataFilterFactory.Create(segmentMetadata, (IAudioParser) this.Parser, new Action<ITrackMetadata>(((MediaParserBase<TConfigurator>) this).SetTrackMetadata), icyMetaInt.Value);
      }
      this._metadataSink.ReportSegmentMetadata(this.Parser.Position ?? TimeSpan.Zero, segmentMetadata);
    }

    public override void SetTrackMetadata(ITrackMetadata trackMetadata)
    {
      this._metadataSink.ReportTrackMetadata(trackMetadata);
    }

    public override void ProcessData(byte[] buffer, int offset, int length)
    {
      Debug.Assert(length > 0);
      Debug.Assert(offset + length <= buffer.Length);
      if (null == this._audioParser)
        throw new InvalidOperationException("StartSegment has not been called");
      this._audioParser.ProcessData(buffer, offset, length);
      this.PushStreams();
    }

    public override void FlushBuffers()
    {
      if (null != this._audioParser)
        this._audioParser.FlushBuffers();
      base.FlushBuffers();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        using (this._audioParser)
          ;
      }
      base.Dispose(disposing);
    }
  }
}
