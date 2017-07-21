using System;
using SM.Media.Core.Audio;
using SM.Media.Core.Audio.Shoutcast;
using SM.Media.Core.MediaParser;
using SM.Media.Core.Metadata;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.Ac3
{
  public sealed class Ac3MediaParser : AudioMediaParser<Ac3Parser, Ac3Configurator>
  {
    private static readonly TsStreamType StreamType = TsStreamType.FindStreamType(TsStreamType.Ac3StreamType);

    public Ac3MediaParser(ITsPesPacketPool pesPacketPool, IShoutcastMetadataFilterFactory shoutcastMetadataFilterFactory, IMetadataSink metadataSink)
      : base(Ac3MediaParser.StreamType, new Ac3Configurator((IMediaStreamMetadata) null, (string) null), pesPacketPool, shoutcastMetadataFilterFactory, metadataSink)
    {
      this.Parser = new Ac3Parser(pesPacketPool, new Action<IAudioFrameHeader>(this.Configurator.Configure), new Action<TsPesPacket>(((MediaParserBase<Ac3Configurator>) this).SubmitPacket));
    }
  }
}
