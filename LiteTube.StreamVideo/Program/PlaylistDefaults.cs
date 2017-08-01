using System.Linq;
using LiteTube.StreamVideo.M3U8;

namespace LiteTube.StreamVideo.Program
{
    public static class PlaylistDefaults
    {
        public static bool IsDynamicPlayist(M3U8Parser parser)
        {
            if (null != M3U8TagInstanceExtensions.Tag(parser.GlobalTags, M3U8Tags.ExtXEndList))
                return false;

            return parser.Playlist.All(p =>
            {
                var extinfTagInstance = M3U8Tags.ExtXInf.Find(p.Tags);
                return extinfTagInstance != null && extinfTagInstance.Duration >= new decimal(0);
            });
        }
    }
}
