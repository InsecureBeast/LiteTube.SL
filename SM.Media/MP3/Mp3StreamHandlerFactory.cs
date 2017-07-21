// Decompiled with JetBrains decompiler
// Type: SM.Media.MP3.Mp3StreamHandlerFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Pes;
using SM.Media.TransportStream.TsParser;
using System.Collections.Generic;

namespace SM.Media.MP3
{
  public class Mp3StreamHandlerFactory : IPesStreamFactoryInstance
  {
    private static readonly byte[] Types = new byte[2]
    {
      TsStreamType.Mp3Iso11172,
      TsStreamType.Mp3Iso13818
    };

    public ICollection<byte> SupportedStreamTypes
    {
      get
      {
        return (ICollection<byte>) Mp3StreamHandlerFactory.Types;
      }
    }

    public PesStreamHandler Create(PesStreamParameters parameters)
    {
      return (PesStreamHandler) new Mp3StreamHandler(parameters);
    }
  }
}
