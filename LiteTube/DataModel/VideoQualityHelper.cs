using LiteTube.LibVideo;

namespace LiteTube.DataModel
{
    class VideoQualityHelper
    {
        public static VideoQuality GetVideoQuality(Multimedia.YouTubeQuality quality)
        {
            switch (quality)
            {
                case Multimedia.YouTubeQuality.Quality144P:
                    return VideoQuality.Quality144P;
                case Multimedia.YouTubeQuality.Quality240P:
                    return VideoQuality.Quality240P;
                case Multimedia.YouTubeQuality.Quality270P:
                    return VideoQuality.Quality270P;
                case Multimedia.YouTubeQuality.Quality360P:
                    return VideoQuality.Quality360P;
                case Multimedia.YouTubeQuality.Quality480P:
                    return VideoQuality.Quality480P;
                case Multimedia.YouTubeQuality.Quality720P:
                    return VideoQuality.Quality720P;
                case Multimedia.YouTubeQuality.Quality1080P:
                    return VideoQuality.Quality1080P;
                default:
                    return VideoQuality.Unknown;
            }
        }
    }
}
