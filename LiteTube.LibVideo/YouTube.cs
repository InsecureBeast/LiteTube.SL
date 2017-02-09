using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LiteTube.LibVideo.Helpers;

namespace LiteTube.LibVideo
{
    public class YouTube
    {
        public static async Task<YouTubeVideo> GetVideoAsync(string videoId, VideoQuality quality)
        {
            var url = $"https://www.youtube.com/watch?v={videoId}&nomobile=1";
            if (!TryNormalize(url, out url))
                throw new ArgumentException("URL is not a valid YouTube URL!");

            var source = await HttpUtils.HttpGetAsync(url, string.Empty);
            var res = await ParseVideosAsync(source);
            var uri = TryFindBestVideoUri(res, VideoQuality.Quality144P, quality);
            return uri;
        }

        private static YouTubeVideo TryFindBestVideoUri(IEnumerable<YouTubeVideo> uris, VideoQuality minQuality, VideoQuality maxQuality)
        {
            var selected = uris.Where(i => IsAvailableVideo(i, minQuality, maxQuality)).ToList();
            var ordered = selected.OrderByDescending(u => u.Resolution).ToList();
            return ordered.FirstOrDefault();
        }

        private static bool IsAvailableVideo(YouTubeVideo video, VideoQuality minQuality, VideoQuality maxQuality)
        {
            return video.AudioFormat != AudioFormat.Unknown 
                && video.Resolution != -1 
                && !video.Is3D 
                && (video.Format == VideoFormat.Mp4 || video.Format == VideoFormat.Mobile) 
                && video.Resolution >= GetResolution(minQuality) 
                && video.Resolution <= GetResolution(maxQuality);
        }

        private static bool TryNormalize(string videoUri, out string normalized)
        {
            // If you fix something in here, please be sure to fix in 
            // DownloadUrlResolver.TryNormalizeYoutubeUrl as well.

            normalized = null;

            var builder = new StringBuilder(videoUri);

            videoUri = builder.Replace("youtu.be/", "youtube.com/watch?v=")
                .Replace("youtube.com/embed/", "youtube.com/watch?v=")
                .Replace("/v/", "/watch?v=")
                .Replace("/watch#", "/watch?")
                .ToString();

            var query = new Query(videoUri);

            string value;

            if (!query.TryGetValue("v", out value))
                return false;

            normalized = "https://youtube.com/watch?v=" + value;
            return true;
        }

        private static async Task<IEnumerable<YouTubeVideo>> ParseVideosAsync(string source)
        {
            var title = Html.GetNode("title", source);
            var jsPlayer = "https://www.youtube.com" + Json.GetKey("js", source).Replace(@"\/", "/");
            var map = Json.GetKey("url_encoded_fmt_stream_map", source);
            var queries = map.Split(',').Select(Unscramble).ToArray();

            if (queries.Any())
                return queries.Select(q => new YouTubeVideo(title, q, jsPlayer));
            
            var adaptiveMap = Json.GetKey("adaptive_fmts", source);

            // If there is no adaptive_fmts key, then in the file
            // will be dashmpd key containing link to a XML
            // file containing links and other data
            if (adaptiveMap == string.Empty)
            {
                using (var hc = new HttpClient())
                {
                    var temp = Json.GetKey("dashmpd", source);
                    temp = WebUtility.UrlDecode(temp).Replace(@"\/", "/");

                    var manifest = await hc.GetStringAsync(temp);
                    manifest = manifest.Replace(@"\/", "/").Replace("%2F", "/");

                    var uris = Html.GetUrisFromManifest(manifest);
                    return uris.Select(u => new YouTubeVideo(title, new UnscrambledQuery(u, false), jsPlayer, true));
                }
            }

            queries = adaptiveMap.Split(',').Select(Unscramble).ToArray();
            return queries.Select(q => new YouTubeVideo(title, q, jsPlayer));
        }

        // TODO: Consider making this static...
        private static UnscrambledQuery Unscramble(string queryString)
        {
            queryString = queryString.Replace(@"\u0026", "&");
            var query = new Query(queryString);
            string uri = query["url"];

            bool encrypted = false;
            string signature;

            if (query.TryGetValue("s", out signature))
            {
                encrypted = true;
                uri += GetSignatureAndHost(signature, query);
            }
            else if (query.TryGetValue("sig", out signature))
                uri += GetSignatureAndHost(signature, query);

            uri = WebUtility.UrlDecode(WebUtility.UrlDecode(uri));

            var uriQuery = new Query(uri);

            if (!uriQuery.ContainsKey("ratebypass"))
                uri += "&ratebypass=yes";

            return new UnscrambledQuery(uri, encrypted);
        }

        private static string GetSignatureAndHost(string signature, Query query)
        {
            var result = "&signature=" + signature;
            
            string host;
            if (query.TryGetValue("fallback_host", out host))
                result += "&fallback_host=" + host;

            return result;
        }

        private static int GetResolution (VideoQuality quality)
        {
            switch (quality)
            {
                case VideoQuality.Unknown:
                    return -1;
                case VideoQuality.Quality144P:
                    return 144;
                case VideoQuality.Quality240P:
                    return 240;
                case VideoQuality.Quality270P:
                    return 270;
                case VideoQuality.Quality360P:
                    return 360;
                case VideoQuality.Quality480P:
                    return 480;
                case VideoQuality.Quality720P:
                    return 720;
                case VideoQuality.Quality1080P:
                    return 1080;
                default:
                    return -1;
            }
        }
    }

    /// <summary>Enumeration of stream qualities. </summary>
    public enum VideoQuality : short
    {
        Unknown,
        Quality144P,
        Quality240P,
        Quality270P,
        Quality360P,
        Quality480P,
        Quality720P,
        Quality1080P,
    }
}
