using System;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.AAC
{
    public class AacStreamHandler : AudioStreamHandler
    {
        private const int MinimumPacketSize = 7;

        public AacStreamHandler(PesStreamParameters parameters)
            : base(parameters, new AacFrameHeader(), new AacConfigurator(parameters.MediaStreamMetadata, parameters.StreamType.Description), 7)
        {
            if (!AacDecoderSettings.Parameters.UseParser)
                return;

            Parser = new AacParser(parameters.PesPacketPool, new Action<IAudioFrameHeader>(AudioConfigurator.Configure), NextHandler);
        }
    }
}
