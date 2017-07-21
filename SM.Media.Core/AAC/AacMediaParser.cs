using System;
using SM.Media.Core.Audio;
using SM.Media.Core.Audio.Shoutcast;
using SM.Media.Core.MediaParser;
using SM.Media.Core.Metadata;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.AAC
{
  public sealed class AacMediaParser : AudioMediaParser<AacParser, AacConfigurator>
  {
    private static readonly TsStreamType StreamType = TsStreamType.FindStreamType(TsStreamType.AacStreamType);

    public AacMediaParser(ITsPesPacketPool pesPacketPool, IShoutcastMetadataFilterFactory shoutcastMetadataFilterFactory, IMetadataSink metadataSink)
      : base(AacMediaParser.StreamType, new AacConfigurator((IMediaStreamMetadata) null, (string) null), pesPacketPool, shoutcastMetadataFilterFactory, metadataSink)
    {
      this.Parser = new AacParser(pesPacketPool, Configurator.Configure, ((MediaParserBase<AacConfigurator>) this).SubmitPacket);
    }
  }
}
