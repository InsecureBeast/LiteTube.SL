using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteTube.StreamVideo.M3U8.AttributeSupport;

namespace LiteTube.StreamVideo.M3U8.TagSupport
{
  public class AttributesTagInstance : M3U8TagInstance
  {
    public IEnumerable<M3U8AttributeInstance> Attributes { get; private set; }

    internal AttributesTagInstance(M3U8Tag tag, IEnumerable<M3U8AttributeInstance> attributes)
      : base(tag)
    {
      this.Attributes = attributes;
    }

    public static M3U8TagInstance Create(M3U8Tag tag, string value, IDictionary<string, M3U8Attribute> attributes)
    {
      return AttributesTagInstance.Create(tag, value, (Func<string, IEnumerable<M3U8AttributeInstance>>) (v => M3U8AttributeParserSupport.ParseAttributes(v, attributes)));
    }

    private static M3U8TagInstance Create(M3U8Tag tag, string value, Func<string, IEnumerable<M3U8AttributeInstance>> attributeParser)
    {
      return (M3U8TagInstance) new AttributesTagInstance(tag, AttributesTagInstance.ParseAttributes(value, attributeParser));
    }

    protected static IEnumerable<M3U8AttributeInstance> ParseAttributes(string value, Func<string, IEnumerable<M3U8AttributeInstance>> attributeParser)
    {
      IEnumerable<M3U8AttributeInstance> enumerable = (IEnumerable<M3U8AttributeInstance>) null;
      if (!string.IsNullOrWhiteSpace(value) && null != attributeParser)
        enumerable = attributeParser(value);
      return enumerable;
    }

    protected static IEnumerable<M3U8AttributeInstance> ParseAttributes(string value, IDictionary<string, M3U8Attribute> attributes)
    {
      return AttributesTagInstance.ParseAttributes(value, (Func<string, IEnumerable<M3U8AttributeInstance>>) (v => M3U8AttributeParserSupport.ParseAttributes(v, attributes)));
    }

    public override string ToString()
    {
      IEnumerable<M3U8AttributeInstance> attributes = this.Attributes;
      if (attributes == null || !Enumerable.Any<M3U8AttributeInstance>(attributes))
        return base.ToString();
      StringBuilder stringBuilder = new StringBuilder(this.Tag.Name);
      stringBuilder.Append(':');
      bool flag = true;
      foreach (M3U8AttributeInstance attributeInstance in attributes)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(',');
        stringBuilder.Append((object) attributeInstance);
      }
      return stringBuilder.ToString();
    }
  }
}
