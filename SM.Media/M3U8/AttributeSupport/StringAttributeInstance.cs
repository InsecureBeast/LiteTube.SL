// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.AttributeSupport.StringAttributeInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Globalization;

namespace SM.Media.M3U8.AttributeSupport
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
