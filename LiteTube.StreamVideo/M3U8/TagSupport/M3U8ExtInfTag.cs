using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.M3U8.TagSupport
{
  public class M3U8ExtInfTag : M3U8Tag
  {
    public M3U8ExtInfTag(string name, M3U8TagScope scope)
      : base(name, scope, new Func<M3U8Tag, string, M3U8TagInstance>(ExtinfTagInstance.Create))
    {
    }

    public ExtinfTagInstance Find(IEnumerable<M3U8TagInstance> tags)
    {
      if (null == tags)
        return (ExtinfTagInstance) null;
      return M3U8TagInstanceExtensions.Tag<M3U8ExtInfTag, ExtinfTagInstance>(tags, this);
    }
  }
}
