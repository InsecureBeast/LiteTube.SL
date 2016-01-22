using LiteTube.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LiteTube.DataModel
{
    class YouTubeResponce
    {
        public IEnumerable<string> Ids { get; set; }
        public string NextPageToken { get; set; }
    }

    class YouTubeWeb
    {
        private const string RECOMMENDED_URL = @"https://www.youtube.com/feed/recommended";
        private const string BOT_USER_AGENT = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        private readonly Dictionary<string, IEnumerable<string>> _recommended = new Dictionary<string, IEnumerable<string>>();
        private readonly Dictionary<string, IEnumerable<string>> _related = new Dictionary<string, IEnumerable<string>>();


        public async Task<YouTubeResponce> GetRecommended(string accessToken, string nextPageToken)
        {
            return await GetVideos(RECOMMENDED_URL, _recommended, accessToken, nextPageToken);
        }

        public async Task<YouTubeResponce> GetRelatedVideo(string videoId, string accessToken, string nextPageToken)
        {
            var url = string.Format("https://www.youtube.com/watch?v={0}", videoId);
            var res = await GetVideos(url, _related, accessToken, nextPageToken);
            if (res == null)
                return null;

            var items = res.Ids.ToList();
            foreach (var item in res.Ids)
            {
                if (item.Contains(videoId))
                    items.Remove(item);
            }
                
            res.Ids = items;
            return res;
        }

        public async Task<YouTubeResponce> GetChannelVideos(string channelId, string accessToken, string nextPageToken)
        {
            var url = string.Format("https://www.youtube.com/channel/{0}/videos", channelId);
            var videos = new Dictionary<string, IEnumerable<string>>();
            return await GetVideos(url, videos, accessToken, nextPageToken);
        }

        private static List<List<string>> Split(IEnumerable<string> source)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 40)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static void OpenVideo(string videoId, string accessToken)
        {
            var url = string.Format("https://www.youtube.com/v/{0}", videoId);
            var responce = HttpGetAsync(url, accessToken);
        }

        public static async Task<IEnumerable<string>> HttpGetAutoCompleteAsync(string query)
        {
            var uri = string.Format("http://suggestqueries.google.com/complete/search?client=youtube&ds=yt&q={0}", query);
            var response = await HttpGetAsync(uri, string.Empty);
            
            if (string.IsNullOrEmpty(response))
                return null;

            var regex = new Regex(@"\[(.*?),0\]");
            var list = new List<string>();
            response = response.Remove(0, 21);
            var colm = regex.Matches(response);
            foreach (Match match in colm)
            {
                //TODO вот эту лабуду бы в regexp'e отбросить. Пока не получилось
                var str = match.Value.Replace("[", "");
                str = str.Replace("\"", "");
                str = str.Replace(",0]", "");
                if (!list.Contains(str))
                    list.Add(str);
            }
            
            return list;
        }

        /// <exception cref="WebException">An error occurred while downloading the resource. </exception>
        private static async Task<string>HttpGetAsync(string uri, string accessToken)
        {
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip };
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", BOT_USER_AGENT);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Add("Authorization", accessToken);

                var response = await client.GetAsync(new Uri(uri, UriKind.Absolute));
                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task<YouTubeResponce> GetVideos(string url, Dictionary<string, IEnumerable<string>> dic, string accessToken, string nextPageToken)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(nextPageToken))
            {
                var response = await HttpGetAsync(url, accessToken);
                var regex = new Regex(@"watch\?v=(.*)");
                var colm = regex.Matches(response);
                foreach (Match match in colm)
                {
                    var end = match.Value.IndexOf("\"", StringComparison.Ordinal);
                    var str = match.Value.Substring(8, end - 8);
                    if (!result.Contains(str))
                        result.Add(str);
                }

                if (result.Count == 0)
                    return null;

                var splitted = Split(result);
                for (int i = 0; i < splitted.Count; i++)
                {
                    //var pageToken = i + 1;
                    dic[i.ToString()] = splitted[i];
                }

                var yt = new YouTubeResponce() { Ids = dic.Values.First(), NextPageToken = "1" };
                return yt;
            }

            IEnumerable<string> value = null;
            if (dic.TryGetValue(nextPageToken, out value))
            {
                var ids = dic[nextPageToken];
                int pageToken = 0;
                if (int.Parse(nextPageToken) < dic.Count)
                {
                    pageToken = int.Parse(nextPageToken);
                    pageToken++;
                }
                var yt = new YouTubeResponce() { Ids = ids, NextPageToken = pageToken.ToString() };
                return yt;
            }

            return null;
        }
    }
}
