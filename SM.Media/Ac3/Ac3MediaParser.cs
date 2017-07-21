// Decompiled with JetBrains decompiler
// Type: SM.Media.Ac3.Ac3MediaParser
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

namespace SM.Media.Ac3
{
  public sealed class Ac3MediaParser : AudioMediaParser<Ac3Parser, Ac3Configurator>
  {
    private static readonly TsStreamType StreamType = TsStreamType.FindStreamType(TsStreamType.Ac3StreamType);

    public Ac3MediaParser(ITsPesPacketPool pesPacketPool, IShoutcastMetadataFilterFactory shoutcastMetadataFilterFactory, IMetadataSink metadataSink)
      : base(Ac3MediaParser.StreamType, new Ac3Configurator((IMediaStreamMetadata) null, (string) null), pesPacketPool, shoutcastMetadataFilterFactory, metadataSink)
    {
      this.Parser = new Ac3Parser(pesPacketPool, new Action<IAudioFrameHeader>(this.Configurator.Configure), new Action<TsPesPacket>(((MediaParserBase<Ac3Configurator>) this).SubmitPacket));
    }
  }
}
