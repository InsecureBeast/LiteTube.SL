using System;
using LiteTube.StreamVideo.M3U8;
using LiteTube.StreamVideo.Program;

namespace LiteTube.StreamVideo.Hls
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
