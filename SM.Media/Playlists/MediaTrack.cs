// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.MediaTrack
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;
using System.Text;

namespace SM.Media.Playlists
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
