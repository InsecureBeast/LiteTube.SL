using System;
using LiteTube.StreamVideo.Audio;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Ac3
{
  public class Ac3StreamHandler : AudioStreamHandler
  {
    private const int MinimumPacketSize = 7;
    private const bool UseParser = true;

    public Ac3StreamHandler(PesStreamParameters parameters)
      : base(parameters, (IAudioFrameHeader) new Ac3FrameHeader(), (IAudioConfigurator) new Ac3Configurator(parameters.MediaStreamMetadata, parameters.StreamType.Description), 7)
    {
      this.Parser = (AudioParserBase) new Ac3Parser(parameters.PesPacketPool, new Action<IAudioFrameHeader>(this.AudioConfigurator.Configure), this.NextHandler);
    }
  }
}
