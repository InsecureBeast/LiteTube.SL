using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube
{
    class BugTreckerReporter
    {
        public static async Task SendException(Exception exception)
        {
            await Send(exception, string.Empty);
        }

        internal static async Task SendException(Exception exception, IEnumerable<string> region)
        {
            var builder = new StringBuilder();
            //0 -10 video
            var ids = region.Take(10);
            foreach (var id in ids)
            {
                builder.AppendLine(id);
                builder.AppendLine(",");
            }

            await Send(exception, builder.ToString());
        }

        private static async Task Send(Exception exception, string videoIds)
        {
            string url = "https://bitbucket.org/api/1.0/repositories/insecureBeast/litetube/issues/";
            var request = WebRequest.Create(url) as HttpWebRequest;

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("insecureBeast" + ":" + "GM9d3Lqw"));
            request.Headers["Authorization"] = "Basic " + credentials;
            request.Method = "POST";
            request.ContentType = @"application/x-www-form-urlencoded";

            var exceptionInfo = BuildExceptionInfo(exception, videoIds);
            var postData = string.Format("title={0}&content={1}&status=new&priority=trivial&kind=bug", exception.Message, exceptionInfo);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            var dataStream = await request.GetRequestStreamAsync();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Dispose();

            using (var response = await request.GetResponseAsync() as HttpWebResponse)
            {
                var reader = new StreamReader(response.GetResponseStream());
                string json = reader.ReadToEnd();
            }
        }

        private static string BuildExceptionInfo(Exception exception, string region)
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine("Exception mesage: " + exception.Message);
            builder.AppendLine();
            builder.AppendLine("Stack trace : " + exception.StackTrace);
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                builder.AppendLine();
                builder.AppendLine("InnerException message: " + exception.Message);
                builder.AppendLine();
                builder.AppendLine("InnerException stack trace: " + exception.StackTrace);
            }

            builder.AppendLine();
            builder.AppendLine("Region: " + region);
            return builder.ToString();
        }
    }
}
