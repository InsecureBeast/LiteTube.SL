// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.TagSupport.DateTimeTagInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Diagnostics;
using System.Globalization;

namespace SM.Media.M3U8.TagSupport
{
  public class DateTimeTagInstance : M3U8TagInstance
  {
    public DateTimeOffset DateTime { get; private set; }

    public DateTimeTagInstance(M3U8Tag tag, DateTimeOffset dateTime)
      : base(tag)
    {
      this.DateTime = dateTime;
    }

    internal static M3U8TagInstance Create(M3U8Tag tag, string value)
    {
      DateTimeOffset result;
      if (DateTimeOffset.TryParse(value, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
        return (M3U8TagInstance) new DateTimeTagInstance(tag, result);
      Debug.WriteLine("*** unable to parse date/time: " + value);
      return (M3U8TagInstance) null;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1:o}", (object) this.Tag, (object) this.DateTime);
    }
  }
}
