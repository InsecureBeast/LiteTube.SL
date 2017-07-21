// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.TextEncodings.AsciiEncoding
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Text;

namespace SM.Media.Utility.TextEncodings
{
  public class AsciiEncoding : Encoding
  {
    public override int GetByteCount(char[] chars, int index, int count)
    {
      return count;
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
      for (int index = 0; index < charCount; ++index)
      {
        char ch = chars[index + charIndex];
        byte num = (int) ch <= (int) sbyte.MaxValue ? (byte) ch : (byte) 63;
        bytes[index + byteIndex] = num;
      }
      return charCount;
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
      return count;
    }

    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
    {
      for (int index = 0; index < byteCount; ++index)
      {
        byte num = bytes[index + byteIndex];
        chars[index + charIndex] = (int) num <= (int) sbyte.MaxValue ? (char) num : '�';
      }
      return byteCount;
    }

    public override int GetMaxByteCount(int charCount)
    {
      return charCount;
    }

    public override int GetMaxCharCount(int byteCount)
    {
      return byteCount;
    }
  }
}
