using System;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.MP3
{
    public class Mp3StreamHandler : AudioStreamHandler
    {
        private const int MINIMUM_PACKET_SIZE = 24;
        
        public Mp3StreamHandler(PesStreamParameters parameters)
          : base(parameters, new Mp3FrameHeader(), new Mp3Configurator(parameters.MediaStreamMetadata, parameters.StreamType.Description), MINIMUM_PACKET_SIZE)
        {
            Parser = new Mp3Parser(parameters.PesPacketPool, AudioConfigurator.Configure, NextHandler);
        }
    }
}
