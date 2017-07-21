// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.TagSupport.M3U8DateTimeTag
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Collections.Generic;

namespace SM.Media.M3U8.TagSupport
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
