using Autofac;
using LiteTube.StreamVideo.Web;
using LiteTube.StreamVideo.Web.HttpConnection;
using LiteTube.StreamVideo.Web.HttpConnectionReader;

namespace LiteTube.StreamVideo.Module
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
