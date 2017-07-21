// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.Rfc2047Encoding
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace SM.Media.Web
{
  public static class Rfc2047Encoding
  {
    private static readonly bool[] NeedsEncodingFlags = new bool[(int) sbyte.MaxValue];

    static Rfc2047Encoding()
    {
      for (char c = char.MinValue; (int) c < Rfc2047Encoding.NeedsEncodingFlags.Length; ++c)
      {
        if (char.IsControl(c) || char.IsWhiteSpace(c) || 61 == (int) c || 34 == (int) c)
          Rfc2047Encoding.NeedsEncodingFlags[(int) c] = true;
      }
    }

    public static bool NeedsRfc2047Encoding(char value)
    {
      if ((int) value >= Rfc2047Encoding.NeedsEncodingFlags.Length)
        return true;
      return Rfc2047Encoding.NeedsEncodingFlags[(int) value];
    }

    public static bool NeedsRfc2047Encoding(string value)
    {
      return Enumerable.Any<char>(Enumerable.Cast<char>((IEnumerable) value), new Func<char, bool>(Rfc2047Encoding.NeedsRfc2047Encoding));
    }

    public static string Rfc2047Encode(this string value)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      if (!Rfc2047Encoding.NeedsRfc2047Encoding(value))
        return value;
      return "=?utf-8?B?" + Convert.ToBase64String(Encoding.UTF8.GetBytes(value)) + "?=";
    }
  }
}
