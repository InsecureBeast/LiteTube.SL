﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.HlsSegmentsFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using SM.Media.Segments;
using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Hls
{
  public class HlsSegmentsFactory : IHlsSegmentsFactory
  {
    private readonly IHlsStreamSegmentsFactory _streamSegmentsFactory;

    public HlsSegmentsFactory(IHlsStreamSegmentsFactory streamSegmentsFactory)
    {
      if (null == streamSegmentsFactory)
        throw new ArgumentNullException("streamSegmentsFactory");
      this._streamSegmentsFactory = streamSegmentsFactory;
    }

    public Task<ICollection<ISegment>> CreateSegmentsAsync(M3U8Parser parser, IWebReader webReader, CancellationToken cancellationToken)
    {
      return this._streamSegmentsFactory.Create(parser, webReader).CreateSegmentsAsync(cancellationToken);
    }
  }
}
