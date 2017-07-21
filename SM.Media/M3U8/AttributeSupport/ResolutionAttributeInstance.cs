// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.AttributeSupport.ResolutionAttributeInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Globalization;

namespace SM.Media.M3U8.AttributeSupport
{
  public sealed class ResolutionAttributeInstance : M3U8AttributeInstance
  {
    private static readonly char[] ResolutionSeparator = new char[2]
    {
      'x',
      'X'
    };
    public readonly int X;
    public readonly int Y;

    public ResolutionAttributeInstance(M3U8Attribute attribute, int x, int y)
      : base(attribute)
    {
      this.X = x;
      this.Y = y;
    }

    public static M3U8AttributeInstance Create(M3U8Attribute attribute, string value)
    {
      int length = value.IndexOfAny(ResolutionAttributeInstance.ResolutionSeparator);
      if (length < 1 || length + 1 >= value.Length)
        return (M3U8AttributeInstance) null;
      int x = int.Parse(value.Substring(0, length), (IFormatProvider) CultureInfo.InvariantCulture);
      int y = int.Parse(value.Substring(length + 1), (IFormatProvider) CultureInfo.InvariantCulture);
      return (M3U8AttributeInstance) new ResolutionAttributeInstance(attribute, x, y);
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}x{2}", (object) this.Attribute.Name, (object) this.X, (object) this.Y);
    }
  }
}
