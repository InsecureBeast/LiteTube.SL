using System;
using SM.Media.Core.M3U8;
using SM.Media.Core.Playlists;

namespace SM.Media.Core.Hls
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
