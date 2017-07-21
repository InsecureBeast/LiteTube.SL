// Decompiled with JetBrains decompiler
// Type: SM.Media.AAC.AacStreamHandler
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.TransportStream.TsParser;
using System;

namespace SM.Media.AAC
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
