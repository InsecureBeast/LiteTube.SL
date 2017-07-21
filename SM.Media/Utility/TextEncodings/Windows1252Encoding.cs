// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.TextEncodings.Windows1252Encoding
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;
using System.Text;

namespace SM.Media.Utility.TextEncodings
{
  public class Windows1252Encoding : Encoding
  {
    private static readonly char[] CharLookup = new char[32]
    {
      '€',
      '�',
      '‚',
      'ƒ',
      '„',
      '…',
      '†',
      '‡',
      '\x02C6',
      '‰',
      'Š',
      '‹',
      'Œ',
      '�',
      'Ž',
      '�',
      '�',
      '‘',
      '’',
      '“',
      '”',
      '•',
      '–',
      '—',
      '˜',
      '™',
      'š',
      '›',
      'œ',
      '�',
      'ž',
      'Ÿ'
    };
    private static readonly Dictionary<char, byte> ByteLookup = new Dictionary<char, byte>(Windows1252Encoding.CharLookup.Length);
    private static readonly int ByteLookupMax;

    static Windows1252Encoding()
    {
      for (int index1 = 0; index1 < Windows1252Encoding.CharLookup.Length; ++index1)
      {
        char index2 = Windows1252Encoding.CharLookup[index1];
        if (65533 != (int) index2)
        {
          if ((int) index2 > Windows1252Encoding.ByteLookupMax)
            Windows1252Encoding.ByteLookupMax = (int) index2;
          Windows1252Encoding.ByteLookup[index2] = (byte) (index1 + 128);
        }
      }
    }

    public override int GetByteCount(char[] chars, int index, int count)
    {
      return count;
    }

    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
    {
      for (int index = 0; index < charCount; ++index)
      {
        char key = chars[index + charIndex];
        byte num;
        if ((int) key < 128)
          num = (byte) key;
        else if ((int) key < 160)
        {
          if (!Windows1252Encoding.ByteLookup.TryGetValue(key, out num))
            num = (byte) 63;
        }
        else
          num = (int) key >= 256 ? (byte) 63 : (byte) key;
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
        char ch = (int) num > 128 && (int) num < 160 ? Windows1252Encoding.CharLookup[(int) num - 128] : (char) num;
        chars[index + charIndex] = ch;
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
