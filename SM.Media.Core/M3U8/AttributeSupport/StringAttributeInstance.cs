using System;
using System.Globalization;

namespace SM.Media.Core.M3U8.AttributeSupport
{
  public class StringAttributeInstance : M3U8AttributeValueInstance<string>
  {
    public StringAttributeInstance(M3U8Attribute attribute, string value)
      : base(attribute, value)
    {
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}=\"{1}\"", (object) this.Attribute.Name, (object) this.Value);
    }
  }
}
