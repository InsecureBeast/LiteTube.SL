// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.TagSupport.ExtinfTagInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using System;
using System.Diagnostics;
using System.Globalization;

namespace SM.Media.M3U8.TagSupport
{
  public sealed class ExtinfTagInstance : M3U8TagInstance
  {
    public Decimal Duration { get; private set; }

    public string Title { get; private set; }

    public ExtinfTagInstance(M3U8Tag tag, Decimal duration, string title = null)
      : base(tag)
    {
      this.Duration = duration;
      this.Title = title ?? string.Empty;
    }

    internal static ExtinfTagInstance Create(M3U8Tag tag, string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        Debug.WriteLine("*** Invalid empty #EXTINF tag");
        return new ExtinfTagInstance(tag, new Decimal(0), (string) null);
      }
      int length = value.IndexOf(',');
      if (length < 0)
        return new ExtinfTagInstance(tag, ExtinfTagInstance.ParseDuration(value), (string) null);
      Decimal duration = ExtinfTagInstance.ParseDuration(value.Substring(0, length));
      string title = string.Empty;
      if (length + 1 < value.Length)
        title = value.Substring(length + 1).Trim();
      return new ExtinfTagInstance(tag, duration, title);
    }

    private static Decimal ParseDuration(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        Debug.WriteLine("*** Invalid #EXTINF duration is empty");
        return new Decimal(0);
      }
      Decimal num = Decimal.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (num <= new Decimal(0) && new Decimal(-1) != num)
        Debug.WriteLine("*** Invalid #EXTINF duration: " + (object) num);
      else if (num > new Decimal(14400))
        Debug.WriteLine("*** Excessive #EXTINF duration?: " + (object) num);
      return num;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1},{2}", (object) this.Tag, (object) this.Duration, (object) this.Title);
    }
  }
}
