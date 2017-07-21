using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SM.Media.Core.M3U8.AttributeSupport
{
  public static class M3U8AttributeSupport
  {
    public static M3U8TagInstance CreateInstance(M3U8Tag tag, string value)
    {
      return new M3U8TagInstance(tag);
    }

    public static M3U8AttributeValueInstance<long> DecimalIntegerParser(M3U8Attribute attribute, string value)
    {
      return new M3U8AttributeValueInstance<long>(attribute, long.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static M3U8AttributeValueInstance<string> QuotedStringParser(M3U8Attribute attribute, string value)
    {
      if (value.Length < 2 || 34 != (int) value[0] || 34 != (int) value[value.Length - 1])
        return (M3U8AttributeValueInstance<string>) null;
      return (M3U8AttributeValueInstance<string>) new StringAttributeInstance(attribute, value.Substring(1, value.Length - 2));
    }

    private static string StripQuotes(string s)
    {
      if (string.IsNullOrWhiteSpace(s))
        return (string) null;
      s = s.Trim();
      if (s.Length < 2 || 34 != (int) s[0] || 34 != (int) s[s.Length - 1])
        return (string) null;
      return s.Substring(1, s.Length - 2);
    }

    public static M3U8AttributeValueInstance<IEnumerable<string>> QuotedCsvParser(M3U8Attribute attribute, string value)
    {
      value = M3U8AttributeSupport.StripQuotes(value);
      string[] strArray = Enumerable.ToArray<string>(Enumerable.Select<string, string>((IEnumerable<string>) value.Split(','), (Func<string, string>) (s => s.Trim())));
      return (M3U8AttributeValueInstance<IEnumerable<string>>) new CsvStringsAttributeInstance(attribute, (IEnumerable<string>) strArray);
    }

    public static M3U8AttributeValueInstance<byte[]> HexadecialIntegerParser(M3U8Attribute attribute, string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return (M3U8AttributeValueInstance<byte[]>) null;
      int num1 = value.IndexOf("0x", StringComparison.OrdinalIgnoreCase);
      if (num1 < 0 || num1 + 2 >= value.Length)
        return (M3U8AttributeValueInstance<byte[]>) null;
      int num2 = num1 + 2;
      List<byte> list = new List<byte>(16);
      int num3 = 0;
      bool flag = false;
      for (int index = num2; index < value.Length; ++index)
      {
        char ch = value[index];
        byte num4;
        if ((int) ch >= 48 && (int) ch <= 57)
          num4 = (byte) ((uint) ch - 48U);
        else if ((int) ch >= 97 && (int) ch <= 102)
          num4 = (byte) ((int) ch - 97 + 10);
        else if ((int) ch >= 65 && (int) ch <= 70)
          num4 = (byte) ((int) ch - 65 + 10);
        else
          continue;
        if (flag)
        {
          list.Add((byte) ((uint) (num3 << 4) | (uint) num4));
          flag = false;
        }
        else
        {
          num3 = (int) num4;
          flag = true;
        }
      }
      if (flag)
      {
        list.Add((byte) (num3 << 4));
        int num4 = 0;
        for (int index = 0; index < list.Count; ++index)
        {
          byte num5 = list[index];
          list[index] = (byte) (num4 << 4 | (int) num5 >> 4);
          num4 = (int) (byte) ((uint) num5 & 15U);
        }
      }
      return (M3U8AttributeValueInstance<byte[]>) new HexadecimalIntegerAttributeInstance(attribute, list.ToArray());
    }
  }
}
