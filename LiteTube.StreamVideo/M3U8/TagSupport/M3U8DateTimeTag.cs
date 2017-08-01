using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.M3U8.TagSupport
{
  public class M3U8DateTimeTag : M3U8Tag
  {
    public M3U8DateTimeTag(string name, M3U8TagScope scope)
      : base(name, scope, new Func<M3U8Tag, string, M3U8TagInstance>(DateTimeTagInstance.Create))
    {
    }

    public DateTimeTagInstance Find(IEnumerable<M3U8TagInstance> tags)
    {
      if (null == tags)
        return (DateTimeTagInstance) null;
      return M3U8TagInstanceExtensions.Tag<M3U8DateTimeTag, DateTimeTagInstance>(tags, this);
    }
  }
}
