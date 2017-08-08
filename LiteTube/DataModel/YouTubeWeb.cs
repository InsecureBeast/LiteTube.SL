using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using LiteTube.DataClasses;

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
        private const string MOST_POPULAR_URL = @"https://www.youtube.com/feed/trending//?gl=";
        private const string SUBSCRIPTIONS_URL = @"https://www.youtube.com/feed/subscriptions/?app=desktop&persist_app=1&flow=2";
        private const string WATCH_LATER_URL = @"https://www.youtube.com/playlist?list=WL&app=desktop&persist_app=1";
        private const string HISTORY_URL = @"https://www.youtube.com/feed/history";
        private const string FEED_URL_FORMAT = @"https://www.youtube.com/feeds/videos.xml?channel_id={0}";


        private const string BOT_USER_AGENT1 = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        private const string BOT_USER_AGENT = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
        private readonly Dictionary<string, IEnumerable<string>> _recommended = new Dictionary<string, IEnumerable<string>>();
        private readonly Dictionary<string, IEnumerable<string>> _related = new Dictionary<string, IEnumerable<string>>();
        private readonly Dictionary<string, IEnumerable<string>> _watchLater = new Dictionary<string, IEnumerable<string>>();
        private readonly Dictionary<string, IEnumerable<string>> _hystory = new Dictionary<string, IEnumerable<string>>();
        private readonly Dictionary<string, IEnumerable<string>> _mostPopular = new Dictionary<string, IEnumerable<string>>();
        private readonly Dictionary<string, IEnumerable<string>> _activity = new Dictionary<string, IEnumerable<string>>();


        public async Task<YouTubeResponce> GetRecommended(string accessToken, string nextPageToken)
        {
            return await GetVideos(RECOMMENDED_URL, _recommended, accessToken, nextPageToken);
        }

        public async Task<YouTubeResponce> GetActivity(IEnumerable<string> subscriptions,  string accessToken, string nextPageToken)
        {
            /*
            var subs = new List<string>()
                {
                    "UCt7sv-NKh44rHAEb-qCCxvA",
                    "UCovLyuOCfHLOPP8Jwc3aZoA",
                    "UCKc2vTLJM0Zt6fPuxvTl9pw",
                    "UCpfefG1t0k2FJ8mevWHhp0g",
                    "UC1c3-bhBuf9brQW-XMUxjnw",
                    "UCf31Gf5nCU8J6eUlr7QSU0w",
                    "UCOOC9ar-ZUCNvWAqr9iAAkg",
                    "UC-iIrGcOLkRcnu5ozK5fYOg",
                    "UCLlSts9lJLf90vFFNx7le4w",
                    "UCwd0QUcZSa5iGjt-v8z5w2g",
                    "UCNb2BkmQu3IfQVcaPExHkvQ",
                    "UCY03gpyR__MuJtBpoSyIGnw",
                    "UCiHkdT46IUqmUgHH0Pxt8aA",
                    "UCgA-kOOYE0W2BY1BZf83cBg",
                    "UC1a2rnuwCw6rEbAxclIHkng",
                    "UCbaxk35aRh1DfXILkdPGukw",
                    "UC5XPnUk8Vvv_pWslhwom6Og",
                    "UCIi2Tk2POJkRgWHD7HGBa7Q",
                    "UCGJLJ7p4jWNwWDY4j9OY8QA",
                    "UC_Q1vhf7wcR_zGlc5ahAg0A",
                    "UCZfYlIsllUFyfmLodIpzU0g",
                    "UCtFbE0nu4pYL8XTZOVC6X7A",
                    "UCPDis9pjXuqyI7RYLJ-TTSA",
                    "UCqU8dYH2dacLT_orXi_nZ2Q",
                    "UCWw6msvCpTDGBKpcPGMDmjA",
                    "UCqYiHHUrS3dm_gE8dy2VMwg",
                    "UCtO0TzSAoIOzTnTsQeywSSw",
                    "UCvNby-vCYhCZEp7gGFGNtBg",
                    "UCbirjI1K3MGu0-Y1gTBNR5w",
                    "UCQBEHg0j6baNS1Lya-L4BJw",
                    "UCVPBbw8E9Kj16mSqE5xi2aQ",
                    "UCDaIW2zPRWhzQ9Hj7a0QP1w",
                    "UC-27_Szq7BtHDoC0R2U0zxA",
                    "UCUZLQoU0DDMhO5CNIUStfug",
                    "UCQeaXcwLUDeRoNVThZXLkmw",
                    "UCTSuE3PvfJ4AWLxuMY2nAbg"
                   
                };
                */
            //return await GetSubscriptionsVideo(subscriptions, accessToken, nextPageToken);
            return await GetVideos(SUBSCRIPTIONS_URL, _activity, accessToken, nextPageToken);
        }

        public async Task<YouTubeResponce> GetSubscriptionsVideo(IEnumerable<string> subscriptions, string accessToken, string nextPageToken)
        {
            if (string.IsNullOrEmpty(nextPageToken))
            {
                var tasks = new List<Task<object>>();
                foreach (var subscription in subscriptions)
                {
                    var task = Task<object>.Factory.StartNew(() =>
                    {
                        try
                        {
                            var url = string.Format(FEED_URL_FORMAT, subscription);
                            var response = HttpGetAsync(url, accessToken).Result;
                            return response;
                        }
                        catch (Exception)
                        {
                            return null;
                        }

                    }, TaskCreationOptions.LongRunning);

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks.ToArray());
                var result = tasks.Where(x => !string.IsNullOrEmpty(x?.Result?.ToString()))
                                  .Select(task => new WebVideo(task.Result.ToString()))
                                  .ToList();

                if (result.Count == 0)
                    return null;

                var entries = new List<Entry>();
                foreach (var webVideo in result)
                {
                    entries.AddRange(webVideo.Feed.Entries);
                }

                //Сортировка по дате
                entries.Sort(new EntryComparer());
                var ids = entries.Select(e => e.VideoId);
                return GetFirstTimeItems(ids, _activity);
            }

            return GetNextItemsFromDictionary(_activity, nextPageToken);
        }

        public async Task<IEnumerable<string>> GetSubscriptions(string accessToken)
        {
            var result = new List<string>();
            var url = "https://www.youtube.com/subscription_manager?action_takeout=1";
            var response = await HttpGetAsync(url, accessToken);
            var regex = new Regex(@"channel_id=([^\""]+)");
            var colm = regex.Matches(response);
            foreach (Match match in colm)
            {
                var str = match.Value.Substring(11, match.Value.Length - 11);
                if (!result.Contains(str))
                    result.Add(str);
            }

            if (result.Count == 0)
                return null;

            return result;
        }

        public async Task<YouTubeResponce> GetWatchLater(string accessToken, string nextPageToken)
        {
            return await GetVideos(WATCH_LATER_URL, _watchLater, accessToken, nextPageToken);
        }

        public async Task<YouTubeResponce> GetHistoryVideo(string accessToken, string nextPageToken)
        {
            return await GetVideos(HISTORY_URL, _hystory, accessToken, nextPageToken);
        }

        public async Task<YouTubeResponce> GetMostPopular(string culture, string nextPageToken)
        {
            return await GetVideos(MOST_POPULAR_URL + culture, _mostPopular, null, nextPageToken);
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
            //var liveChannelId = "UC4R8DWoMoI7CAwX8_LjQHig";
            //if (channelId == liveChannelId)
            //    url = $"https://www.youtube.com/playlist?list=PLU12uITxBEPFb0yuTkLH2tu8j5SVx1YaA";

            var url = $"https://www.youtube.com/channel/{channelId}/videos";
            var videos = new Dictionary<string, IEnumerable<string>>();
            var list = await GetVideos(url, videos, accessToken, nextPageToken);

            if (list != null)
                return list;

            url = $"https://www.youtube.com/channel/{channelId}";
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
            var list = new List<string>();
            var uri = $"http://suggestqueries.google.com/complete/search?client=youtube&ds=yt&q={query}";
            var response = await HttpGetAsync(uri, string.Empty);
            
            if (string.IsNullOrEmpty(response))
                return list;

            var regex = new Regex(@"\[(.*?),0\]");
            
            response = response.Remove(0, 21);
            var colm = regex.Matches(response);
            foreach (Match match in colm)
            {
                //TODO вот эту лабуду бы в regexp'e отбросить. Пока не получилось
                var str = match.Value.Replace("[", "");
                str = str.Replace("\"", "");
                str = str.Replace(",0]", "");
                str = Regex.Replace(str, @"[\\u0000-\\u007F]+", string.Empty);
                if (!list.Contains(str))
                    list.Add(str);
            }
            
            return list;
        }

        /// <exception cref="WebException">An error occurred while downloading the resource. </exception>
        public static async Task<string>HttpGetAsync(string uri, string accessToken)
        {
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip };
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", BOT_USER_AGENT);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Add("Authorization", accessToken);

                var response = await client.GetByteArrayAsync(new Uri(uri, UriKind.Absolute));
                var responseString = Encoding.UTF8.GetString(response, 0, response.Length - 1);
                return responseString;
            }
        }

        private async Task<YouTubeResponce> GetVideos(string url, Dictionary<string, IEnumerable<string>> dic, string accessToken, string nextPageToken)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(nextPageToken))
            {
                var response = await HttpGetAsync(url, accessToken);
                var regex = new Regex(@"watch\?v=([^\""|><&\;]+)");
                var colm = regex.Matches(response);
                foreach (Match match in colm)
                {
                    var str = match.Value.Substring(8, match.Value.Length - 8);
                    if (!result.Contains(str))
                        result.Add(str);
                }

                if (result.Count == 0)
                    return null;

                var yt = GetFirstTimeItems(result, dic);
                return yt;
            }

            return GetNextItemsFromDictionary(dic, nextPageToken);
        }

        private YouTubeResponce GetNextItemsFromDictionary(IReadOnlyDictionary<string, IEnumerable<string>> dic, string nextPageToken)
        {
            IEnumerable<string> value = null;
            if (!dic.TryGetValue(nextPageToken, out value))
                return null;

            var ids = dic[nextPageToken];
            int pageToken = 0;
            if (int.Parse(nextPageToken) < dic.Count)
            {
                pageToken = int.Parse(nextPageToken);
                pageToken++;
            }
            var yt = new YouTubeResponce() {Ids = ids, NextPageToken = pageToken.ToString()};
            return yt;
        }

        private YouTubeResponce GetFirstTimeItems(IEnumerable<string> values, Dictionary<string, IEnumerable<string>> dic)
        {
            dic.Clear();
            var splitted = Split(values);
            for (var i = 0; i < splitted.Count; i++)
            {
                //var pageToken = i + 1;
                dic[i.ToString()] = splitted[i];
            }

            var yt = new YouTubeResponce() { Ids = dic.Values.First(), NextPageToken = "1" };
            return yt;
        }
    }
}
