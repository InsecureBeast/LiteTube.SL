using System;

namespace SM.Media.Core.Metadata
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
