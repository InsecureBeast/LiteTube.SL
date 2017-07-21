using System.Text;

namespace SM.Media.Core.M3U8.AttributeSupport
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
