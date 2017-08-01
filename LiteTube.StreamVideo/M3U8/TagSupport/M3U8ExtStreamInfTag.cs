using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.M3U8.TagSupport
{
  public class M3U8ExtStreamInfTag : M3U8Tag
  {
    public M3U8ExtStreamInfTag(string name, M3U8TagScope scope)
      : base(name, scope, new Func<M3U8Tag, string, M3U8TagInstance>(ExtStreamInfTagInstance.Create))
    {
    }

    public ExtStreamInfTagInstance Find(IEnumerable<M3U8TagInstance> tags)
    {
      if (null == tags)
        return (ExtStreamInfTagInstance) null;
      return M3U8TagInstanceExtensions.Tag<M3U8ExtStreamInfTag, ExtStreamInfTagInstance>(tags, this);
    }
  }
}
