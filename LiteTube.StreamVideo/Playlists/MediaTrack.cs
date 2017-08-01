using System;
using System.Text;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Playlists
{
  public class MediaTrack
  {
    public Uri Url { get; set; }

    public string Title { get; set; }

    public bool UseNativePlayer { get; set; }

    public ContentType ContentType { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(this.Title))
      {
        stringBuilder.Append('"');
        stringBuilder.Append(this.Title);
        stringBuilder.Append('"');
      }
      if ((Uri) null != this.Url)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(' ');
        stringBuilder.Append((string) (object) '<' + (object) this.Url.OriginalString + (string) (object) '>');
      }
      if (this.UseNativePlayer)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(' ');
        stringBuilder.Append("[native]");
      }
      return stringBuilder.ToString();
    }
  }
}
