using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.Core.M3U8.AttributeSupport
{
  internal static class ExtMapSupport
  {
    public static readonly M3U8ValueAttribute<string> AttrUri = new M3U8ValueAttribute<string>("URI", true, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8Attribute AttrByterange = new M3U8Attribute("BYTERANGE", false, new Func<M3U8Attribute, string, M3U8AttributeInstance>(ByterangeAttributeInstance.Create));
    internal static readonly IDictionary<string, M3U8Attribute> Attributes = (IDictionary<string, M3U8Attribute>) Enumerable.ToDictionary<M3U8Attribute, string>((IEnumerable<M3U8Attribute>) new M3U8Attribute[2]
    {
      (M3U8Attribute) ExtMapSupport.AttrUri,
      ExtMapSupport.AttrByterange
    }, (Func<M3U8Attribute, string>) (a => a.Name));
  }
}
