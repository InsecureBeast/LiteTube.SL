using System.Collections.Generic;

namespace LiteTube.StreamVideo.M3U8
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
