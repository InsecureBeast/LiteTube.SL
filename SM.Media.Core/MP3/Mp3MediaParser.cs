using System;
using SM.Media.Core.Audio;
using SM.Media.Core.Audio.Shoutcast;
using SM.Media.Core.MediaParser;
using SM.Media.Core.Metadata;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.MP3
{
  public sealed class Mp3MediaParser : AudioMediaParser<Mp3Parser, Mp3Configurator>
  {
    private static readonly TsStreamType StreamType = TsStreamType.FindStreamType(TsStreamType.Mp3Iso11172);

    public Mp3MediaParser(ITsPesPacketPool pesPacketPool, IShoutcastMetadataFilterFactory shoutcastMetadataFilterFactory, IMetadataSink metadataSink)
      : base(Mp3MediaParser.StreamType, new Mp3Configurator((IMediaStreamMetadata) null, (string) null), pesPacketPool, shoutcastMetadataFilterFactory, metadataSink)
    {
      this.Parser = new Mp3Parser(pesPacketPool, new Action<IAudioFrameHeader>(this.Configurator.Configure), new Action<TsPesPacket>(((MediaParserBase<Mp3Configurator>) this).SubmitPacket));
    }
  }
}
