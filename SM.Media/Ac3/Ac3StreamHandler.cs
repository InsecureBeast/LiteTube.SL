// Decompiled with JetBrains decompiler
// Type: SM.Media.Ac3.Ac3StreamHandler
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.TransportStream.TsParser;
using System;

namespace SM.Media.Ac3
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
