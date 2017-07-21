using System;
using SM.Media.Core.Audio;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.MP3
{
  public class Mp3StreamHandler : AudioStreamHandler
  {
    private const int MinimumPacketSize = 24;
    private const bool UseParser = true;

    public Mp3StreamHandler(PesStreamParameters parameters)
      : base(parameters, (IAudioFrameHeader) new Mp3FrameHeader(), (IAudioConfigurator) new Mp3Configurator(parameters.MediaStreamMetadata, parameters.StreamType.Description), 24)
    {
      this.Parser = (AudioParserBase) new Mp3Parser(parameters.PesPacketPool, new Action<IAudioFrameHeader>(this.AudioConfigurator.Configure), this.NextHandler);
    }
  }
}
