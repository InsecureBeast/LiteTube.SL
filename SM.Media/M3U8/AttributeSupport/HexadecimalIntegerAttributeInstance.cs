// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.AttributeSupport.HexadecimalIntegerAttributeInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System.Text;

namespace SM.Media.M3U8.AttributeSupport
{
  internal class HexadecimalIntegerAttributeInstance : M3U8AttributeValueInstance<byte[]>
  {
    public HexadecimalIntegerAttributeInstance(M3U8Attribute attribute, byte[] value)
      : base(attribute, value)
    {
    }

    public override string ToString()
    {
      byte[] numArray = this.Value;
      StringBuilder stringBuilder = new StringBuilder(this.Attribute.Name, this.Attribute.Name.Length + 3 + numArray.Length * 2);
      stringBuilder.Append("=0x");
      foreach (byte num in numArray)
        stringBuilder.Append(num.ToString("X2"));
      return stringBuilder.ToString();
    }
  }
}
