using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LiteTube.LibVideo.Helpers
{
    class HttpUtils
    {
        private const string BOT_USER_AGENT1 = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        private const string BOT_USER_AGENT = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";

        /// <exception cref="WebException">An error occurred while downloading the resource. </exception>
        public static async Task<string> HttpGetAsync(string uri, string accessToken = null)
        {
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip };
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", BOT_USER_AGENT1);
                if (!string.IsNullOrEmpty(accessToken))
                    client.DefaultRequestHeaders.Add("Authorization", accessToken);
                
                var response = await client.GetAsync(new Uri(uri, UriKind.Absolute));
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
