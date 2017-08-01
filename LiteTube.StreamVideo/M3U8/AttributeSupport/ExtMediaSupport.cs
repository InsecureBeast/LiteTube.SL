using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteTube.StreamVideo.M3U8.AttributeSupport
{
  public static class ExtMediaSupport
  {
    public static readonly M3U8ValueAttribute<string> AttrUri = new M3U8ValueAttribute<string>("URI", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<string> AttrType = new M3U8ValueAttribute<string>("TYPE", false, (Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>) ((tag, value) => new M3U8AttributeValueInstance<string>(tag, value)));
    public static readonly M3U8ValueAttribute<string> AttrGroupId = new M3U8ValueAttribute<string>("GROUP-ID", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<string> AttrLanguage = new M3U8ValueAttribute<string>("LANGUAGE", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<string> AttrName = new M3U8ValueAttribute<string>("NAME", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>(M3U8AttributeSupport.QuotedStringParser));
    public static readonly M3U8ValueAttribute<string> AttrDefault = new M3U8ValueAttribute<string>("DEFAULT", false, (Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>) ((tag, value) => new M3U8AttributeValueInstance<string>(tag, value)));
    public static readonly M3U8ValueAttribute<string> AttrAutoselect = new M3U8ValueAttribute<string>("AUTOSELECT", false, (Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>) ((tag, value) => new M3U8AttributeValueInstance<string>(tag, value)));
    public static readonly M3U8ValueAttribute<string> AttrForced = new M3U8ValueAttribute<string>("FORCED", false, (Func<M3U8Attribute, string, M3U8AttributeValueInstance<string>>) ((tag, value) => new M3U8AttributeValueInstance<string>(tag, value)));
    public static readonly M3U8ValueAttribute<IEnumerable<string>> AttrCharacteristics = new M3U8ValueAttribute<IEnumerable<string>>("CHARACTERISTICS", false, new Func<M3U8Attribute, string, M3U8AttributeValueInstance<IEnumerable<string>>>(M3U8AttributeSupport.QuotedCsvParser));
    internal static readonly IDictionary<string, M3U8Attribute> Attributes = (IDictionary<string, M3U8Attribute>) Enumerable.ToDictionary<M3U8Attribute, string>((IEnumerable<M3U8Attribute>) new M3U8Attribute[9]
    {
      (M3U8Attribute) ExtMediaSupport.AttrUri,
      (M3U8Attribute) ExtMediaSupport.AttrType,
      (M3U8Attribute) ExtMediaSupport.AttrGroupId,
      (M3U8Attribute) ExtMediaSupport.AttrLanguage,
      (M3U8Attribute) ExtMediaSupport.AttrName,
      (M3U8Attribute) ExtMediaSupport.AttrDefault,
      (M3U8Attribute) ExtMediaSupport.AttrAutoselect,
      (M3U8Attribute) ExtMediaSupport.AttrForced,
      (M3U8Attribute) ExtMediaSupport.AttrCharacteristics
    }, (Func<M3U8Attribute, string>) (a => a.Name));
  }
}
