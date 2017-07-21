using System;
using System.Collections.Generic;

namespace SM.Media.Core.M3U8.TagSupport
{
  public class M3U8ByterangeTag : M3U8Tag
  {
    public M3U8ByterangeTag(string name, M3U8TagScope scope)
      : base(name, scope, new Func<M3U8Tag, string, M3U8TagInstance>(ByterangeTagInstance.Create))
    {
    }

    public ByterangeTagInstance Find(IEnumerable<M3U8TagInstance> tags)
    {
      if (null == tags)
        return (ByterangeTagInstance) null;
      return M3U8TagInstanceExtensions.Tag<M3U8ByterangeTag, ByterangeTagInstance>(tags, this);
    }
  }
}
