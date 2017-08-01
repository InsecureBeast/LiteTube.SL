using System;
using System.Globalization;

namespace LiteTube.StreamVideo.M3U8.AttributeSupport
{
  public class M3U8AttributeValueInstance<TValue> : M3U8AttributeInstance
  {
    public readonly TValue Value;

    public M3U8AttributeValueInstance(M3U8Attribute attribute, TValue value)
      : base(attribute)
    {
      this.Value = value;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) this.Attribute.Name, (object) this.Value);
    }
  }
}
