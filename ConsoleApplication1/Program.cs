using LiteTube.LibVideo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.LibVideo.Helpers;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = Task.Factory.StartNew(async () =>
            {
                string url = "https://www.youtube.com/browse_ajax?action_continuation=1&continuation=4qmFsgJ9Eg9GRXdoYXRfdG9fd2F0Y2gaaENCQjZTa05wYjBGQlNFb3hRVUZHVTFaUlFVSlZiRlZCUVZGQ1IxSllaRzlaV0ZKbVpFYzVabVF5UmpCWk1tZEJRVkZCUVVGUlJVSkJRVUZDUVVGRlVVTkNhWEZ6U2pkRWNsbDJWRUZuQgA%253D&target_id=section-list-336445&direct_render=1";
                var source = await HttpUtils.HttpGetAsync(url);
                var u = 0;
            });
            //t.Wait();
            //var r = t.Result;

            //string videoId = "i9AHJkHqkpw";

            /*
            var t = Task.Factory.StartNew(async () =>
            {

                var video = await YouTube.GetVideoAsync(videoId, VideoQuality.Quality360P);
                var url = await video.GetUriAsync();
            });
            var r = t.Result;
            */
            Console.ReadKey();
        }
    }
}
