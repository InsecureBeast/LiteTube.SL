// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.PlaylistSubStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Playlists
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
