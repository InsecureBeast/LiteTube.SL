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
            builder.RegisterType<StreamSocketWrapper>().As<ISocket>().ExternallyOwned();
            builder.RegisterType<HttpConnection>().As<IHttpConnection>().ExternallyOwned();
            builder.RegisterType<HttpConnectionFactory>().As<IHttpConnectionFactory>().SingleInstance();
            builder.RegisterType<HttpConnectionRequestFactory>().As<IHttpConnectionRequestFactory>().SingleInstance();
            builder.RegisterType<HttpConnectionRequestFactoryParameters>().As<IHttpConnectionRequestFactoryParameters>().SingleInstance();
            builder.RegisterType<HttpConnectionWebReaderManager>().As<IWebReaderManager>().SingleInstance();
            builder.RegisterType<HttpEncoding>().As<IHttpEncoding>().SingleInstance();
            builder.RegisterType<HttpHeaderSerializer>().As<IHttpHeaderSerializer>().SingleInstance();
            builder.RegisterType<UserAgentEncoder>().As<IUserAgentEncoder>().SingleInstance();
        }
    }
}
