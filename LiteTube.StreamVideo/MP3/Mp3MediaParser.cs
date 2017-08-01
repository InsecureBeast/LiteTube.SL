using System;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.Audio.Shoutcast;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;

namespace LiteTube.StreamVideo.MP3
{
    public sealed class Mp3MediaParser : AudioMediaParser<Mp3Parser, Mp3Configurator>
    {
        private static readonly TsStreamType StreamType = TsStreamType.FindStreamType(TsStreamType.Mp3Iso11172);

        public Mp3MediaParser(ITsPesPacketPool pesPacketPool, IShoutcastMetadataFilterFactory shoutcastMetadataFilterFactory, IMetadataSink metadataSink)
          : base(StreamType, new Mp3Configurator(null), pesPacketPool, shoutcastMetadataFilterFactory, metadataSink)
        {
            Parser = new Mp3Parser(pesPacketPool, Configurator.Configure, SubmitPacket);
        }
    }
}
