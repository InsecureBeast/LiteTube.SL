﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaParser.MediaParserFactoryFinder
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.MediaParser
{
  public class MediaParserFactoryFinder : ContentServiceFactoryFinder<IMediaParser, IMediaParserParameters>, IMediaParserFactoryFinder, IContentServiceFactoryFinder<IMediaParser, IMediaParserParameters>
  {
    public MediaParserFactoryFinder(IEnumerable<IMediaParserFactoryInstance> factoryInstances)
      : base(Enumerable.OfType<IContentServiceFactoryInstance<IMediaParser, IMediaParserParameters>>((IEnumerable) factoryInstances))
    {
    }

    public void Register(ContentType contentType, IMediaParserFactoryInstance factory)
    {
      this.Register(contentType, (IContentServiceFactoryInstance<IMediaParser, IMediaParserParameters>) factory);
    }
  }
}
