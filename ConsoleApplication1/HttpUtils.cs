using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LiteTube.LibVideo.Helpers
{
    class HttpUtils
    {
        private const string BOT_USER_AGENT = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        private const string BOT_USER_AGENT1 = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

        /// <exception cref="WebException">An error occurred while downloading the resource. </exception>
        public static async Task<string> HttpGetAsync(string uri, string accessToken = null)
        {
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip, UseCookies = true};
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", BOT_USER_AGENT1);
                client.DefaultRequestHeaders.Add("X-XSS-Protection", "0");
                
                
                    //if (!string.IsNullOrEmpty(accessToken))
                    //    client.DefaultRequestHeaders.Add("Authorization", accessToken);

                    var response = await client.GetAsync(new Uri(uri, UriKind.Absolute));

                var res = await response.Content.ReadAsStringAsync();
                return res;
            }
        }
    }
}
