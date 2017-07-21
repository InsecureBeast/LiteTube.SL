// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.HlsPlaylistParameters
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using SM.Media.Playlists;
using System;

namespace SM.Media.Hls
{
  public class HlsPlaylistParameters
  {
    private TimeSpan _excessiveDuration = TimeSpan.FromMinutes(5.0);
    private TimeSpan _maximumReload = TimeSpan.FromMinutes(2.0);
    private TimeSpan _minimumReload = TimeSpan.FromSeconds(5.0);
    private TimeSpan _minimumRetry = TimeSpan.FromMilliseconds(333.0);
    private Func<M3U8Parser, bool> _isDynamicPlaylist;

    public TimeSpan MinimumReload
    {
      get
      {
        return this._minimumReload;
      }
      set
      {
        this._minimumReload = value;
      }
    }

    public TimeSpan MaximumReload
    {
      get
      {
        return this._maximumReload;
      }
      set
      {
        this._maximumReload = value;
      }
    }

    public TimeSpan ExcessiveDuration
    {
      get
      {
        return this._excessiveDuration;
      }
      set
      {
        this._excessiveDuration = value;
      }
    }

    public TimeSpan MinimumRetry
    {
      get
      {
        return this._minimumRetry;
      }
      set
      {
        this._minimumRetry = value;
      }
    }

    public Func<M3U8Parser, bool> IsDynamicPlaylist
    {
      get
      {
        return this._isDynamicPlaylist ?? new Func<M3U8Parser, bool>(PlaylistDefaults.IsDynamicPlayist);
      }
      set
      {
        this._isDynamicPlaylist = value;
      }
    }
  }
}
