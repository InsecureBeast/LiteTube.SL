using LiteTube.LibVideo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.LibVideo.Helpers;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Factory.StartNew(async () =>
            {
                var source = new YouTubeWeb();
                var subs = new List<string>()
                {
                    "UCt7sv-NKh44rHAEb-qCCxvA",
                   /* "UCovLyuOCfHLOPP8Jwc3aZoA",
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
                    "UCTSuE3PvfJ4AWLxuMY2nAbg"*/
                   
                }; //Wilsa, Room Factory
                var startTime = DateTime.Now;
                Console.WriteLine(startTime);
                var video = await source.GetSubscriptionsVideo(subs, String.Empty, String.Empty);
                Console.WriteLine(DateTime.Now - startTime);
                Console.WriteLine(video.Count());
                foreach (var o in video.ToList())
                {
                    foreach (var item in o.Items)
                    {
                        Console.WriteLine("Channel ID = " + item.ChannelId);
                        Console.WriteLine("Channel title = " + item.ChannelTitle);
                        foreach (var entry in item.Entries)
                        {
                            Console.WriteLine("VideoID = " + entry.VideoId);
                            Console.WriteLine("Video title = " + entry.Title);
                        }
                    }
                }
            });
            
            
            
            //Console.WriteLine(NumberToStringConverter.Convert(22134567891));
            //Console.WriteLine(NumberToStringConverter.Convert(2213456789));
            //Console.WriteLine(NumberToStringConverter.Convert(221345678));
            //Console.WriteLine(NumberToStringConverter.Convert(22134567));
            //Console.WriteLine(NumberToStringConverter.Convert(2213456));
            //Console.WriteLine(NumberToStringConverter.Convert(221345));
            //Console.WriteLine(NumberToStringConverter.Convert(22213));
            //Console.WriteLine(NumberToStringConverter.Convert(5658));
            //Console.WriteLine(NumberToStringConverter.Convert(658));
            //Console.WriteLine(NumberToStringConverter.Convert(58));
            //Console.WriteLine(NumberToStringConverter.Convert(8));
            //var t = Task.Factory.StartNew(async () =>
            //{
            //    string url = "https://www.youtube.com/browse_ajax?action_continuation=1&continuation=4qmFsgJ9Eg9GRXdoYXRfdG9fd2F0Y2gaaENCQjZTa05wYjBGQlNFb3hRVUZHVTFaUlFVSlZiRlZCUVZGQ1IxSllaRzlaV0ZKbVpFYzVabVF5UmpCWk1tZEJRVkZCUVVGUlJVSkJRVUZDUVVGRlVVTkNhWEZ6U2pkRWNsbDJWRUZuQgA%253D&target_id=section-list-336445&direct_render=1";
            //    var source = await HttpUtils.HttpGetAsync(url);
            //    var u = 0;
            //});
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

    public class NumberToStringConverter 
    {
        public static object Convert(object value)
        {
            if (value == null)
                return string.Empty;

            return $"{value:n0}";
            /*//CultureInfo.CurrentCulture.NumberFormat
            var result = value.ToString();
            for (int i = result.Length; i > 0; i--)
            {
                if ((i) % 3 == 0)
                    result = result.Insert(i+1, " ");
            }
            //if (result.Length > 3)
            //{
            //    result = result.Insert(result.Length - 3, " ");
            //}

            return result;
            */
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
