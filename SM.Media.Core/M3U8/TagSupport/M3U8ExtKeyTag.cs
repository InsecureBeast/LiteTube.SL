using System;
using System.Collections.Generic;

namespace SM.Media.Core.M3U8.TagSupport
{
  public class M3U8ExtKeyTag : M3U8Tag
  {
    public M3U8ExtKeyTag(string name, M3U8TagScope scope)
      : base(name, scope, new Func<M3U8Tag, string, M3U8TagInstance>(ExtKeyTagInstance.Create))
    {
    }

    public IEnumerable<ExtKeyTagInstance> FindAll(IEnumerable<M3U8TagInstance> tags)
    {
      if (null == tags)
        return (IEnumerable<ExtKeyTagInstance>) null;
      return M3U8TagInstanceExtensions.Tags<M3U8ExtKeyTag, ExtKeyTagInstance>(tags, this);
    }
  }
}
