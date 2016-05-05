using System.Collections.Generic;
using System.Linq;
using LiteTube.Multimedia;

namespace LiteTube.Common
{
    internal class VideoQuality
    {
        public const string DEFAULT_QUALITY_NAME = "360p";
        private readonly Dictionary<string, YouTubeQuality> _quality;

        public VideoQuality()
        {
            _quality = new Dictionary<string, YouTubeQuality>
            {
                //{ "144p", YouTubeQuality.Quality144P },
                //{ "240p", YouTubeQuality.Quality240P },
                { "360p", YouTubeQuality.Quality360P },
                { "480p", YouTubeQuality.Quality480P },
                { "720p", YouTubeQuality.Quality720P },
                { "1080p", YouTubeQuality.Quality1080P }
            };
        }

        public YouTubeQuality GetQualityEnum(string qualityName)
        {
            YouTubeQuality qualityEnum;
            if (_quality.TryGetValue(qualityName, out qualityEnum))
                return qualityEnum;

            return YouTubeQuality.Quality270P;
        }

        public IEnumerable<string> GetQualityNames()
        {
            return _quality.Keys;
        }

        public string GetQualityName(YouTubeQuality qualityEnum)
        {
            var name = _quality.FirstOrDefault(d => d.Value == qualityEnum);
            return name.Key;
        }
    }
}
