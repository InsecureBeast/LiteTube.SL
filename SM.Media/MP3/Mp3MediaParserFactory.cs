// Decompiled with JetBrains decompiler
// Type: SM.Media.MP3.Mp3MediaParserFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.MediaParser;
using System;
using System.Collections.Generic;

namespace SM.Media.MP3
{
  public class Mp3MediaParserFactory : MediaParserFactoryBase<Mp3MediaParser>
  {
    private static readonly ContentType[] Types = new ContentType[1]
    {
      ContentTypes.Mp3
    };

    public override ICollection<ContentType> KnownContentTypes
    {
      get
      {
        return (ICollection<ContentType>) Mp3MediaParserFactory.Types;
      }
    }

    public Mp3MediaParserFactory(Func<Mp3MediaParser> factory)
      : base(factory)
    {
    }
  }
}
