using System;
using System.Collections.Generic;
using System.Linq;
using SM.Media.Core.Content;
using SM.Media.Core.Web;

namespace SM.Media.Core.Segments
{
  public class SimpleSegmentManager : SimpleSegmentManagerBase
  {
    public SimpleSegmentManager(IWebReader webReader, IEnumerable<Uri> urls, ContentType contentType)
      : base(webReader, (ICollection<ISegment>) Enumerable.ToArray<ISegment>(Enumerable.Select<Uri, ISegment>(urls, (Func<Uri, ISegment>) (url => (ISegment) new SimpleSegment(url, webReader == null ? (Uri) null : webReader.RequestUri ?? webReader.BaseAddress)))), contentType)
    {
    }
  }
}
