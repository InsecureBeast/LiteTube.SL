// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.TextEncodings.EncodingHelpers
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace LiteTube.StreamVideo.Utility.TextEncodings
{
  public static class EncodingHelpers
  {
    public const char ReplacementCharacter = '�';

    public static bool HasSurrogate(char[] chars, int charIndex, int charCount)
    {
      for (int index = charIndex; index < charCount + charIndex; ++index)
      {
        if (char.IsSurrogate(chars[index]))
          return true;
      }
      return false;
    }

    public static IEnumerable<int> CodePoints(char[] chars, int charIndex, int charCount)
    {
      char? highSurrogate = new char?();
      for (int i = charIndex; i < charCount + charIndex; ++i)
      {
        char c = chars[i];
        if (highSurrogate.HasValue)
        {
          if (char.IsSurrogatePair(highSurrogate.Value, c))
            yield return char.ConvertToUtf32(highSurrogate.Value, c);
          else
            yield return 63;
          highSurrogate = new char?();
        }
        else if (char.IsSurrogate(c))
          highSurrogate = new char?(c);
        else
          yield return (int) c;
      }
    }
  }
}
