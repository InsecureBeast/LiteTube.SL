using System;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.Audio.Shoutcast;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;

namespace LiteTube.StreamVideo.Ac3
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
