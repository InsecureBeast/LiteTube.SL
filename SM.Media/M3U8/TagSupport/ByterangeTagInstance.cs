// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.TagSupport.ByterangeTagInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Globalization;

namespace SM.Media.M3U8.TagSupport
{
  public sealed class ByterangeTagInstance : M3U8TagInstance
  {
    public long Length { get; private set; }

    public long? Offset { get; private set; }

    public ByterangeTagInstance(M3U8Tag tag, long length, long? offset)
      : base(tag)
    {
      this.Length = length;
      this.Offset = offset;
    }

    internal static M3U8TagInstance Create(M3U8Tag tag, string value)
    {
      int length1 = value.IndexOf('@');
      if (length1 < 0 || length1 + 1 >= value.Length)
        return (M3U8TagInstance) new ByterangeTagInstance(tag, long.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture), new long?());
      long length2 = long.Parse(value.Substring(0, length1), (IFormatProvider) CultureInfo.InvariantCulture);
      long num = long.Parse(value.Substring(length1 + 1), (IFormatProvider) CultureInfo.InvariantCulture);
      return (M3U8TagInstance) new ByterangeTagInstance(tag, length2, new long?(num));
    }

    public override string ToString()
    {
      if (this.Offset.HasValue)
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}@{2}", (object) this.Tag, (object) this.Length, (object) this.Offset.Value);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) this.Tag, (object) this.Length);
    }
  }
}
