// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.AttributeSupport.M3U8AttributeValueInstance`1
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Globalization;

namespace SM.Media.M3U8.AttributeSupport
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
