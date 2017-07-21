using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Web.ClientReader
{
    public interface IHttpClientFactoryParameters
    {
        Uri Referrer { get; }

        ICredentials Credentials { get; }

        CookieContainer CookieContainer { get; }
    }
}
