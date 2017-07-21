// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.TrackMetadata
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Metadata
{
  public class TrackMetadata : ITrackMetadata
  {
    public TimeSpan? TimeStamp { get; set; }

    public string Title { get; set; }

    public string Album { get; set; }

    public string Artist { get; set; }

    public int? Year { get; set; }

    public string Genre { get; set; }

    public Uri Website { get; set; }

    public override string ToString()
    {
      string str1 = string.IsNullOrWhiteSpace(this.Title) ? "<null>" : (string) (object) '"' + (object) this.Title + (string) (object) '"';
      TimeSpan? timeStamp = this.TimeStamp;
      string str2;
      if (!timeStamp.HasValue)
      {
        str2 = "<null>";
      }
      else
      {
        timeStamp = this.TimeStamp;
        str2 = timeStamp.ToString();
      }
      string str3 = str2;
      return "Track " + str1 + " @ " + str3;
    }
  }
}
