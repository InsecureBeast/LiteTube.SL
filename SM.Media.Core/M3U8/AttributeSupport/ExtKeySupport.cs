using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.Core.M3U8.AttributeSupport
{
  public static class ExtKeySupport
  {
    public static readonly M3U8ValueAttribute<string> AttrMethod = new M3U8ValueAttribute<string>("METHOD", true, (Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>) ((tag, value) => new M3U8AttributeValueInstance<string>(tag, value)));
    public static readonly M3U8ValueAttribute<string> AttrUri = new M3U8ValueAttribute<string>("URI", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<byte[]> AttrIv = new M3U8ValueAttribute<byte[]>("IV", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<byte[]>>(M3U8AttributeSupport.HexadecialIntegerParser));
    public static readonly M3U8ValueAttribute<string> AttrKeyFormat = new M3U8ValueAttribute<string>("KEYFORMAT", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<string> AttrKeyFormatVersions = new M3U8ValueAttribute<string>("KEYFORMATVERSIONS", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    internal static readonly IDictionary<string, M3U8Attribute> Attributes = (IDictionary<string, M3U8Attribute>) Enumerable.ToDictionary<M3U8Attribute, string>((IEnumerable<M3U8Attribute>) new M3U8Attribute[5]
    {
      (M3U8Attribute) ExtKeySupport.AttrMethod,
      (M3U8Attribute) ExtKeySupport.AttrUri,
      (M3U8Attribute) ExtKeySupport.AttrIv,
      (M3U8Attribute) ExtKeySupport.AttrKeyFormat,
      (M3U8Attribute) ExtKeySupport.AttrKeyFormatVersions
    }, (Func<M3U8Attribute, string>) (a => a.Name));
  }
}
