using System;
using SM.Media.Core.M3U8;
using SM.Media.Core.Playlists;

namespace SM.Media.Core.Hls
{
    public class HlsPlaylistParameters
    {
        private Func<M3U8Parser, bool> _isDynamicPlaylist;

        public TimeSpan MinimumReload { get; set; } = TimeSpan.FromSeconds(5.0);

        public TimeSpan MaximumReload { get; set; } = TimeSpan.FromMinutes(2.0);

        public TimeSpan ExcessiveDuration { get; set; } = TimeSpan.FromMinutes(5.0);

        public TimeSpan MinimumRetry { get; set; } = TimeSpan.FromMilliseconds(333.0);

        public Func<M3U8Parser, bool> IsDynamicPlaylist
        {
            get
            {
                return _isDynamicPlaylist ?? PlaylistDefaults.IsDynamicPlayist;
            }
            set
            {
                _isDynamicPlaylist = value;
            }
        }
    }
}
