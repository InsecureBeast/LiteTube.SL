using System;
using System.Collections.Generic;

namespace SM.Media.Core.M3U8.TagSupport
{
  public class M3U8ValueTag : M3U8Tag
  {
    public M3U8ValueTag(string name, M3U8TagScope scope, Func<M3U8Tag, string, ValueTagInstance> createInstance)
      : base(name, scope, (Func<M3U8Tag, string, M3U8TagInstance>) ((tag, value) => (M3U8TagInstance) createInstance(tag, value)))
    {
    }

    public ValueTagInstance Find(IEnumerable<M3U8TagInstance> tags)
    {
      if (null == tags)
        return (ValueTagInstance) null;
      return M3U8TagInstanceExtensions.Tag<M3U8ValueTag, ValueTagInstance>(tags, this);
    }

    public T? GetValue<T>(IEnumerable<M3U8TagInstance> tags) where T : struct
    {
      ValueTagInstance valueTagInstance = this.Find(tags);
      if (null == valueTagInstance)
        return new T?();
      return new T?((T) valueTagInstance.Value);
    }

    public T GetObject<T>(IEnumerable<M3U8TagInstance> tags) where T : class
    {
      ValueTagInstance valueTagInstance = this.Find(tags);
      if (null == valueTagInstance)
        return default (T);
      return (T) valueTagInstance.Value;
    }
  }
}
