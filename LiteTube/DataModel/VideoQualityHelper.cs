namespace LiteTube.DataModel
{
    class VideoQualityHelper
    {
        public static VideoLibrary.VideoQuality GetVideoQuality(Multimedia.YouTubeQuality quality)
        {
            switch (quality)
            {
                case Multimedia.YouTubeQuality.Quality144P:
                    return VideoLibrary.VideoQuality.Quality144P;
                case Multimedia.YouTubeQuality.Quality240P:
                    return VideoLibrary.VideoQuality.Quality240P;
                case Multimedia.YouTubeQuality.Quality270P:
                    return VideoLibrary.VideoQuality.Quality270P;
                case Multimedia.YouTubeQuality.Quality360P:
                    return VideoLibrary.VideoQuality.Quality360P;
                case Multimedia.YouTubeQuality.Quality480P:
                    return VideoLibrary.VideoQuality.Quality480P;
                case Multimedia.YouTubeQuality.Quality720P:
                    return VideoLibrary.VideoQuality.Quality720P;
                case Multimedia.YouTubeQuality.Quality1080P:
                    return VideoLibrary.VideoQuality.Quality1080P;
                default:
                    return VideoLibrary.VideoQuality.Unknown;
            }
        }
    }
}
