﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.H264StreamHandlerFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Pes;
using SM.Media.TransportStream.TsParser;
using System.Collections.Generic;

namespace SM.Media.H264
{
  public class H264StreamHandlerFactory : IPesStreamFactoryInstance
  {
    private static readonly byte[] Types = new byte[1]
    {
      TsStreamType.H264StreamType
    };

    public ICollection<byte> SupportedStreamTypes
    {
      get
      {
        return (ICollection<byte>) H264StreamHandlerFactory.Types;
      }
    }

    public PesStreamHandler Create(PesStreamParameters parameters)
    {
      return (PesStreamHandler) new H264StreamHandler(parameters);
    }
  }
}
