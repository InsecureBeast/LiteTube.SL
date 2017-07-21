using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using SM.Media.Core.Web;
using SM.Media.Core.Web.HttpConnection;
using SM.Media.Core.Web.HttpConnectionReader;

namespace SM.Media.Core.Module
{
    public class HttpConnectionModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegistrationExtensions.RegisterType<StreamSocketWrapper>(builder).As<ISocket>().ExternallyOwned();
            RegistrationExtensions.RegisterType<HttpConnection>(builder).As<IHttpConnection>().ExternallyOwned();
            RegistrationExtensions.RegisterType<HttpConnectionFactory>(builder).As<IHttpConnectionFactory>().SingleInstance();
            RegistrationExtensions.RegisterType<HttpConnectionRequestFactory>(builder).As<IHttpConnectionRequestFactory>().SingleInstance();
            RegistrationExtensions.RegisterType<HttpConnectionRequestFactoryParameters>(builder).As<IHttpConnectionRequestFactoryParameters>().SingleInstance();
            RegistrationExtensions.RegisterType<HttpConnectionWebReaderManager>(builder).As<IWebReaderManager>().SingleInstance();
            RegistrationExtensions.RegisterType<HttpEncoding>(builder).As<IHttpEncoding>().SingleInstance();
            RegistrationExtensions.RegisterType<HttpHeaderSerializer>(builder).As<IHttpHeaderSerializer>().SingleInstance();
            RegistrationExtensions.RegisterType<UserAgentEncoder>(builder).As<IUserAgentEncoder>().SingleInstance();
        }
    }
}
