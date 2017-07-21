// Decompiled with JetBrains decompiler
// Type: SM.Media.AAC.AacMediaParser
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.Audio.Shoutcast;
using SM.Media.MediaParser;
using SM.Media.Metadata;
using SM.Media.TransportStream.TsParser;
using SM.Media.TransportStream.TsParser.Utility;
using System;

namespace SM.Media.AAC
{
  public sealed class AacMediaParser : AudioMediaParser<AacParser, AacConfigurator>
  {
    private static readonly TsStreamType StreamType = TsStreamType.FindStreamType(TsStreamType.AacStreamType);

    public AacMediaParser(ITsPesPacketPool pesPacketPool, IShoutcastMetadataFilterFactory shoutcastMetadataFilterFactory, IMetadataSink metadataSink)
      : base(AacMediaParser.StreamType, new AacConfigurator((IMediaStreamMetadata) null, (string) null), pesPacketPool, shoutcastMetadataFilterFactory, metadataSink)
    {
      this.Parser = new AacParser(pesPacketPool, new Action<IAudioFrameHeader>(this.Configurator.Configure), new Action<TsPesPacket>(((MediaParserBase<AacConfigurator>) this).SubmitPacket));
    }
  }
}
