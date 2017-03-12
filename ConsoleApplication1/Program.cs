using LiteTube.LibVideo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //string videoId = "https://youtube.com/watch?v=i9AHJkHqkpw";
            string videoId = "i9AHJkHqkpw";
            var t = Task.Factory.StartNew(async () =>
            {
                var video = await YouTube.GetVideoAsync(videoId, VideoQuality.Quality360P);
                var url = await video.GetUriAsync();
            });
            var r = t.Result;
            Console.ReadKey();
        }
    }
}
