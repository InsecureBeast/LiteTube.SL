using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary.Helpers;

namespace VideoLibrary
{
    public class YouTube
    {
        public async static Task<YouTubeVideo> GetVideoAsync(string videoId, VideoQuality quality)
        {
            var url = string.Format("https://www.youtube.com/watch?v={0}&nomobile=1", videoId);
            if (!TryNormalize(url, out url))
                throw new ArgumentException("URL is not a valid YouTube URL!");

            string source = await HttpUtils.HttpGetAsync(url, string.Empty);
            var res = ParseVideos(source).ToList();
            var uri = TryFindBestVideoUri(res, VideoQuality.Quality144P, quality);
            return uri;
        }

        private static YouTubeVideo TryFindBestVideoUri(IEnumerable<YouTubeVideo> uris, VideoQuality minQuality, VideoQuality maxQuality)
        {
            var selected = uris.Where(i => i.AudioFormat != AudioFormat.Unknown &&
                                    i.Resolution != -1 &&
                                    !i.Is3D &&
                                    (i.Format == VideoFormat.Mp4 || i.Format == VideoFormat.Mobile) &&
                                    i.Resolution >= GetResolution(minQuality) &&
                                    i.Resolution <= GetResolution(maxQuality)).ToList();
            var ordered = selected.OrderByDescending(u => u.Resolution).ToList();
            return ordered.FirstOrDefault();
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

        private static IEnumerable<YouTubeVideo> ParseVideos(string source)
        {
            string title = Html.GetNode("title", source);

            string jsPlayer = "http:" + Json.GetKey("js", source).Replace(@"\/", "/");

            string map = Json.GetKey("url_encoded_fmt_stream_map", source);
            var queries = map.Split(',').Select(Unscramble);

            foreach (var query in queries)
                yield return new YouTubeVideo(title, query, jsPlayer);

            string adaptiveMap = Json.GetKey("adaptive_fmts", source);

            // If there is no adaptive_fmts key, then in the file
            // will be dashmpd key containing link to a XML
            // file containing links and other data
            if (adaptiveMap == String.Empty)
            {
                using (HttpClient hc = new HttpClient())
                {
                    string temp = Json.GetKey("dashmpd", source);
                    temp = WebUtility.UrlDecode(temp).Replace(@"\/", "/");

                    var manifest = hc.GetStringAsync(temp)
                        .GetAwaiter().GetResult()
                        .Replace(@"\/", "/")
                        .Replace("%2F", "/");

                    var uris = Html.GetUrisFromManifest(manifest);

                    foreach (var v in uris)
                    {
                        yield return new YouTubeVideo(title,
                            new UnscrambledQuery(v, false),
                            jsPlayer, true);
                    }
                }
            }
            else queries = adaptiveMap.Split(',').Select(Unscramble);

            foreach (var query in queries)
                yield return new YouTubeVideo(title, query, jsPlayer);
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
            string result = "&signature=" + signature;

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
