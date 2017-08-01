using System;
using System.Diagnostics;
using System.Globalization;

namespace LiteTube.StreamVideo.M3U8.TagSupport
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
