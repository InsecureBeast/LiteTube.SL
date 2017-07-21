using System;
using System.Globalization;

namespace SM.Media.Core.M3U8.AttributeSupport
{
  public sealed class ByterangeAttributeInstance : M3U8AttributeInstance
  {
    public readonly long Length;
    public readonly long? Offset;

    public ByterangeAttributeInstance(M3U8Attribute attribute, long length, long? offset)
      : base(attribute)
    {
      this.Length = length;
      this.Offset = offset;
    }

    public static M3U8AttributeInstance Create(M3U8Attribute attribute, string value)
    {
      int length1 = value.IndexOf('@');
      if (length1 < 0 || length1 + 1 >= value.Length)
        return (M3U8AttributeInstance) new ByterangeAttributeInstance(attribute, long.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture), new long?());
      long length2 = long.Parse(value.Substring(0, length1), (IFormatProvider) CultureInfo.InvariantCulture);
      long num = long.Parse(value.Substring(length1 + 1), (IFormatProvider) CultureInfo.InvariantCulture);
      return (M3U8AttributeInstance) new ByterangeAttributeInstance(attribute, length2, new long?(num));
    }

    public override string ToString()
    {
      if (this.Offset.HasValue)
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}@{2}", (object) this.Attribute, (object) this.Length, (object) this.Offset.Value);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) this.Attribute, (object) this.Length);
    }
  }
}
