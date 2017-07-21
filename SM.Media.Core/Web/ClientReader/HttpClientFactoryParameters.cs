using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Web.ClientReader
{
    public class HttpClientFactoryParameters : IHttpClientFactoryParameters
    {
        public Uri Referrer { get; set; }

        public ICredentials Credentials { get; set; }

        public CookieContainer CookieContainer { get; set; }
    }
}
