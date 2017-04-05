using LiteTube.LibVideo;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            Console.WriteLine(NumberToStringConverter.Convert(22134567891));
            Console.WriteLine(NumberToStringConverter.Convert(2213456789));
            Console.WriteLine(NumberToStringConverter.Convert(221345678));
            Console.WriteLine(NumberToStringConverter.Convert(22134567));
            Console.WriteLine(NumberToStringConverter.Convert(2213456));
            Console.WriteLine(NumberToStringConverter.Convert(221345));
            Console.WriteLine(NumberToStringConverter.Convert(22213));
            Console.WriteLine(NumberToStringConverter.Convert(5658));
            Console.WriteLine(NumberToStringConverter.Convert(658));
            Console.WriteLine(NumberToStringConverter.Convert(58));
            Console.WriteLine(NumberToStringConverter.Convert(8));
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
