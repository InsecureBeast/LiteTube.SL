using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SM.Media.Core.Content;

namespace SM.Media.Core.Web.ClientReader
{
    public interface IHttpClientFactory : IDisposable
    {
        HttpClient CreateClient(Uri baseAddress, Uri referrer = null, ContentKind contentKind = ContentKind.Unknown, ContentType contentType = null);
    }
}
