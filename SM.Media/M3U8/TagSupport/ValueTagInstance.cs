// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.TagSupport.ValueTagInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Globalization;

namespace SM.Media.M3U8.TagSupport
{
  public sealed class ValueTagInstance : M3U8TagInstance
  {
    public object Value { get; private set; }

    private ValueTagInstance(M3U8Tag tag, object value)
      : base(tag)
    {
      this.Value = value;
    }

    internal static ValueTagInstance Create(M3U8Tag tag, string value, Func<string, object> valueParser)
    {
      return new ValueTagInstance(tag, valueParser(value));
    }

    internal static ValueTagInstance CreateLong(M3U8Tag tag, string value)
    {
      return ValueTagInstance.Create(tag, value, (Func<string, object>) (v => (object) long.Parse(v, (IFormatProvider) CultureInfo.InvariantCulture)));
    }

    public override string ToString()
    {
      if (null == this.Value)
        return base.ToString();
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) this.Tag, this.Value);
    }
  }
}
