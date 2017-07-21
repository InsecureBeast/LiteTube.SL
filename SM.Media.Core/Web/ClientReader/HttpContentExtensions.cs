using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Web.ClientReader
{
    public static class HttpContentExtensions
    {
        public static string FileName(this HttpContent httpContent)
        {
            if (null == httpContent)
                return (string)null;
            ContentDispositionHeaderValue contentDisposition = httpContent.Headers.ContentDisposition;
            if (null == contentDisposition)
                return (string)null;
            return contentDisposition.FileName;
        }
    }
}
