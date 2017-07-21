﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.TsProgramMapTableFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.TransportStream.TsParser.Descriptor;
using System;

namespace SM.Media.TransportStream.TsParser
{
  public class TsProgramMapTableFactory : ITsProgramMapTableFactory
  {
    private readonly ITsDescriptorFactory _descriptorFactory;

    public TsProgramMapTableFactory(ITsDescriptorFactory descriptorFactory)
    {
      if (null == descriptorFactory)
        throw new ArgumentNullException("descriptorFactory");
      this._descriptorFactory = descriptorFactory;
    }

    public TsProgramMapTable Create(ITsDecoder decoder, int programNumber, uint pid, Action<IProgramStreams> streamFilter)
    {
      return new TsProgramMapTable(decoder, this._descriptorFactory, programNumber, pid, streamFilter);
    }
  }
}
