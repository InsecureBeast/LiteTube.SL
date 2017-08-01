using System;

namespace LiteTube.StreamVideo.Playlists
{
  public class PlaylistSubStream : SubStream
  {
    public string Type { get; set; }

    public string GroupId { get; set; }

    public bool IsDefault { get; set; }

    public string Language { get; set; }

    public bool IsAutoselect { get; set; }

    public Uri Playlist { get; set; }
  }
}
