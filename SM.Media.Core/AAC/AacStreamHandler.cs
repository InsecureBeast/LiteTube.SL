using System;
using SM.Media.Core.Audio;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.AAC
{
  public class AacStreamHandler : AudioStreamHandler
  {
    private const int MinimumPacketSize = 7;

    public AacStreamHandler(PesStreamParameters parameters)
      : base(parameters, (IAudioFrameHeader) new AacFrameHeader(), (IAudioConfigurator) new AacConfigurator(parameters.MediaStreamMetadata, parameters.StreamType.Description), 7)
    {
      if (!AacDecoderSettings.Parameters.UseParser)
        return;
      this.Parser = (AudioParserBase) new AacParser(parameters.PesPacketPool, new Action<IAudioFrameHeader>(this.AudioConfigurator.Configure), this.NextHandler);
    }
  }
}
