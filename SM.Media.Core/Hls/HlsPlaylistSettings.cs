using SM.Media.Core.Utility;

namespace SM.Media.Core.Hls
{
    public static class HlsPlaylistSettings
    {
        private static readonly ResettableParameters<HlsPlaylistParameters> _playlistParameters = new ResettableParameters<HlsPlaylistParameters>();

        public static HlsPlaylistParameters Parameters
        {
            get
            {
                return _playlistParameters.Parameters;
            }
            set
            {
                _playlistParameters.Parameters = value;
            }
        }
    }
}
