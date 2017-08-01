using System;
using System.Collections.Generic;
using System.Linq;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Segments
{
  public class SimpleSegmentManager : SimpleSegmentManagerBase
  {
    public SimpleSegmentManager(IWebReader webReader, IEnumerable<Uri> urls, ContentType contentType)
      : base(webReader, (ICollection<ISegment>) Enumerable.ToArray<ISegment>(Enumerable.Select<Uri, ISegment>(urls, (Func<Uri, ISegment>) (url => (ISegment) new SimpleSegment(url, webReader == null ? (Uri) null : webReader.RequestUri ?? webReader.BaseAddress)))), contentType)
    {
    }
  }
}
