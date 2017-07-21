// Decompiled with JetBrains decompiler
// Type: SM.Media.Content.ContentTypes
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.Content
{
  public static class ContentTypes
  {
    public static readonly ContentType Mp3 = new ContentType("MP3", ContentKind.Audio, "audio/mpeg", ".mp3", (IEnumerable<string>) new string[3]
    {
      "audio/mpeg3",
      "audio/x-mpeg-3",
      "audio/x-mp3"
    });
    public static readonly ContentType Aac = new ContentType("AAC", ContentKind.Audio, "audio/aac", ".aac", (IEnumerable<string>) new string[1]
    {
      "audio/aacp"
    });
    public static readonly ContentType Ac3 = new ContentType("AC3", ContentKind.Audio, "audio/ac3", ".ac3", (IEnumerable<string>) null);
    public static readonly ContentType TransportStream = new ContentType("MPEG-2 Transport Stream", ContentKind.Container, "video/MP2T", ".ts", (IEnumerable<string>) null);
    public static readonly ContentType M3U8 = new ContentType("M3U8", ContentKind.Playlist, "application/vnd.apple.mpegurl", ".m3u8", (IEnumerable<string>) new string[1]
    {
      "application/x-mpegURL"
    });
    public static readonly ContentType M3U = new ContentType("M3U", ContentKind.Playlist, "audio/mpegURL", ".m3u", (IEnumerable<string>) null);
    public static readonly ContentType Pls = new ContentType("PLS", ContentKind.Playlist, "audio/x-scpls", ".pls", (IEnumerable<string>) null);
    public static readonly ContentType H262 = new ContentType("H.262/MPEG-2", ContentKind.Video, "video/mpeg", ".mpg", (IEnumerable<string>) null);
    public static readonly ContentType H264 = new ContentType("H.264/MPEG-4", ContentKind.Video, "video/mp4", ".mp4", (IEnumerable<string>) null);
    public static readonly ContentType Html = new ContentType("HTML", ContentKind.Other, "text/html", ".html", (IEnumerable<string>) new string[1]
    {
      "application/xhtml+xml"
    });
    public static readonly ContentType Binary = new ContentType("Binary", ContentKind.Other, "application/octet-stream", ".bin", (IEnumerable<string>) null);
    private static readonly ContentType[] AllContentTypes = new ContentType[11]
    {
      ContentTypes.Mp3,
      ContentTypes.Aac,
      ContentTypes.Ac3,
      ContentTypes.TransportStream,
      ContentTypes.M3U8,
      ContentTypes.M3U,
      ContentTypes.Pls,
      ContentTypes.H262,
      ContentTypes.H264,
      ContentTypes.Html,
      ContentTypes.Binary
    };

    public static IEnumerable<ContentType> AllTypes
    {
      get
      {
        return (IEnumerable<ContentType>) ContentTypes.AllContentTypes;
      }
    }
  }
}
