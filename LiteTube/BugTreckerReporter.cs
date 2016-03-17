using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using LiteTube.Common.Helpers;
using Google;
using LiteTube.ViewModels.Nodes;
using Microsoft.Phone.Info;

namespace LiteTube
{
    class BugTreckerReporter
    {
        public static async Task SendException(Exception exception)
        {
            await Send(exception, string.Empty);
        }

        internal static async Task SendException(Exception exception, IEnumerable<string> param)
        {
            try
            {
                var builder = new StringBuilder();
                builder.AppendFormat("OS version - {0}{1}", Environment.OSVersion, Environment.NewLine);
                builder.AppendFormat("Device name - {0}{1}", DeviceStatus.DeviceManufacturer + " " + DeviceStatus.DeviceName, Environment.NewLine);
                builder.AppendFormat("App version - {0}{1}", Assembly.GetExecutingAssembly().GetName().Version, Environment.NewLine);
                builder.AppendFormat("Region - {0}{1}", SettingsHelper.GetRegion(), Environment.NewLine);
                builder.AppendFormat("Quality - {0}{1}", SettingsHelper.GetQuality(), Environment.NewLine);
                builder.AppendFormat("Is Authorized - {0}{1}", SettingsHelper.IsContainsAuthorizationData(), Environment.NewLine);
                foreach (var p in param)
                {
                    builder.AppendFormat("param = {0}{1}", p, Environment.NewLine);
                }

                await Send(exception, builder.ToString());
            }
            catch (Exception)
            {
                ;
            }
        }

        private static async Task Send(Exception exception, string addInfo)
        {
            const string url = "https://bitbucket.org/api/1.0/repositories/insecureBeast/litetubesl/issues/";
            var request = WebRequest.Create(url) as HttpWebRequest;

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("insecureBeast" + ":" + "GM9d3Lqw"));
            request.Headers["Authorization"] = "Basic " + credentials;
            request.Method = "POST";
            request.ContentType = @"application/x-www-form-urlencoded";

            var exceptionInfo = BuildExceptionInfo(exception, addInfo);
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

        private static string BuildExceptionInfo(Exception exception, string addInfo)
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
            builder.AppendLine("Additional info:");
            builder.AppendLine(addInfo);
            return builder.ToString();
        }
    }
}
