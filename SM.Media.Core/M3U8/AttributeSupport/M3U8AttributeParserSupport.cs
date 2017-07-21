using System;
using System.Collections.Generic;
using System.Text;

namespace SM.Media.Core.M3U8.AttributeSupport
{
  internal static class M3U8AttributeParserSupport
  {
    private static readonly M3U8AttributeInstance[] NoAttributes = new M3U8AttributeInstance[0];
    private static readonly char[] PostEqualsChars = new char[2]
    {
      ',',
      '"'
    };

    internal static IEnumerable<M3U8AttributeInstance> ParseAttributes(string value, IDictionary<string, M3U8Attribute> attributes)
    {
      if (string.IsNullOrWhiteSpace(value))
        return (IEnumerable<M3U8AttributeInstance>) M3U8AttributeParserSupport.NoAttributes;
      int startIndex1 = 0;
      int index1 = 0;
      StringBuilder stringBuilder = new StringBuilder();
      List<M3U8AttributeInstance> list = new List<M3U8AttributeInstance>();
      while (index1 < value.Length)
      {
        int num1 = value.IndexOf('=', startIndex1);
        if (num1 < 0)
          num1 = value.Length;
        stringBuilder.Length = 0;
        for (int index2 = startIndex1; index2 < num1; ++index2)
        {
          char ch = value[index2];
          if ((int) ch >= 65 && (int) ch <= 90 || 45 == (int) ch)
            stringBuilder.Append(ch);
        }
        string key = stringBuilder.ToString();
        int startIndex2 = num1 + 1;
        index1 = value.IndexOfAny(M3U8AttributeParserSupport.PostEqualsChars, startIndex2);
        if (index1 < 0)
          index1 = value.Length;
        if (index1 < value.Length && 34 == (int) value[index1])
        {
          int num2 = value.IndexOf('"', index1 + 1);
          if (num2 >= 0)
          {
            index1 = value.IndexOf(',', num2 + 1);
            if (index1 < 0)
              index1 = value.Length;
          }
          else
            break;
        }
        if (index1 > startIndex2)
        {
          string str = value.Substring(startIndex2, index1 - startIndex2);
          startIndex1 = Math.Min(index1 + 1, value.Length);
          M3U8Attribute m3U8Attribute;
          if (attributes.TryGetValue(key, out m3U8Attribute))
          {
            M3U8AttributeInstance attributeInstance = m3U8Attribute.CreateInstance(m3U8Attribute, str);
            if (null != attributeInstance)
              list.Add(attributeInstance);
          }
        }
        else
          break;
      }
      if (list.Count < 1)
        return (IEnumerable<M3U8AttributeInstance>) M3U8AttributeParserSupport.NoAttributes;
      return (IEnumerable<M3U8AttributeInstance>) list;
    }
  }
}
