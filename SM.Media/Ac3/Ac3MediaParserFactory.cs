// Decompiled with JetBrains decompiler
// Type: SM.Media.Ac3.Ac3MediaParserFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.MediaParser;
using System;
using System.Collections.Generic;

namespace SM.Media.Ac3
{
  public class Ac3MediaParserFactory : MediaParserFactoryBase<Ac3MediaParser>
  {
    private static readonly ContentType[] Types = new ContentType[1]
    {
      ContentTypes.Ac3
    };

    public override ICollection<ContentType> KnownContentTypes
    {
      get
      {
        return (ICollection<ContentType>) Ac3MediaParserFactory.Types;
      }
    }

    public Ac3MediaParserFactory(Func<Ac3MediaParser> factory)
      : base(factory)
    {
    }
  }
}
