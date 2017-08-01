using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.Audio.Shoutcast;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;

namespace LiteTube.StreamVideo.AAC
{
    public sealed class AacMediaParser : AudioMediaParser<AacParser, AacConfigurator>
    {
        private static readonly TsStreamType StreamType = TsStreamType.FindStreamType(TsStreamType.AacStreamType);

        public AacMediaParser(ITsPesPacketPool pesPacketPool, IShoutcastMetadataFilterFactory shoutcastMetadataFilterFactory, IMetadataSink metadataSink)
            : base(StreamType, new AacConfigurator((IMediaStreamMetadata) null, null), pesPacketPool, shoutcastMetadataFilterFactory, metadataSink)
        {
            Parser = new AacParser(pesPacketPool, Configurator.Configure, SubmitPacket);
        }
    }
}
