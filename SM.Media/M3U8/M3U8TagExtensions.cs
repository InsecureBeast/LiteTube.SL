// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.M3U8TagExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.M3U8
{
  public static class M3U8TagExtensions
  {
    private static readonly IDictionary<string, M3U8Attribute> NoAttributes = (IDictionary<string, M3U8Attribute>) new Dictionary<string, M3U8Attribute>();

    public static IDictionary<string, M3U8Attribute> Attributes(this M3U8Tag tag)
    {
      M3U8AttributeTag m3U8AttributeTag = tag as M3U8AttributeTag;
      if ((M3U8Tag) null == (M3U8Tag) m3U8AttributeTag)
        return M3U8TagExtensions.NoAttributes;
      return m3U8AttributeTag.Attributes;
    }
  }
}
