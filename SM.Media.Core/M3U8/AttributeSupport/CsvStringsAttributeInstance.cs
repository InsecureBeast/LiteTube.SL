using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace SM.Media.Core.M3U8.AttributeSupport
{
  internal class CsvStringsAttributeInstance : M3U8AttributeValueInstance<IEnumerable<string>>
  {
    public CsvStringsAttributeInstance(M3U8Attribute attribute, IEnumerable<string> codecs)
      : base(attribute, codecs)
    {
    }

    public override string ToString()
    {
      IEnumerable<string> enumerable = this.Value;
      if (null == enumerable)
      {
        Debug.Assert(false);
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}=\"{1}\"", (object) this.Attribute.Name, (object) this.Value);
      }
      StringBuilder stringBuilder = new StringBuilder(this.Attribute.Name);
      stringBuilder.Append("=\"");
      bool flag = true;
      foreach (string str in enumerable)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(", ");
        stringBuilder.Append(str);
      }
      stringBuilder.Append('"');
      return stringBuilder.ToString();
    }
  }
}
