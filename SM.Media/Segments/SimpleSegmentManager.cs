// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.SimpleSegmentManager
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.Segments
{
  public class SimpleSegmentManager : SimpleSegmentManagerBase
  {
    public SimpleSegmentManager(IWebReader webReader, IEnumerable<Uri> urls, ContentType contentType)
      : base(webReader, (ICollection<ISegment>) Enumerable.ToArray<ISegment>(Enumerable.Select<Uri, ISegment>(urls, (Func<Uri, ISegment>) (url => (ISegment) new SimpleSegment(url, webReader == null ? (Uri) null : webReader.RequestUri ?? webReader.BaseAddress)))), contentType)
    {
    }
  }
}
