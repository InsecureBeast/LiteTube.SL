// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.TsProgramAssociationTableFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.TransportStream.TsParser
{
  public class TsProgramAssociationTableFactory : ITsProgramAssociationTableFactory
  {
    private readonly ITsProgramMapTableFactory _programMapTableFactory;

    public TsProgramAssociationTableFactory(ITsProgramMapTableFactory programMapTableFactory)
    {
      if (null == programMapTableFactory)
        throw new ArgumentNullException("programMapTableFactory");
      this._programMapTableFactory = programMapTableFactory;
    }

    public TsProgramAssociationTable Create(ITsDecoder decoder, Func<int, bool> programFilter, Action<IProgramStreams> streamFilter)
    {
      return new TsProgramAssociationTable(decoder, this._programMapTableFactory, programFilter, streamFilter);
    }
  }
}
