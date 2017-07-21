// Decompiled with JetBrains decompiler
// Type: SM.Media.MP3.Mp3StreamHandler
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.TransportStream.TsParser;
using System;

namespace SM.Media.MP3
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
