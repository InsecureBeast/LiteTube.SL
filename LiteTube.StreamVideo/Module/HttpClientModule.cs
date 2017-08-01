using System.Net.Http;
using Autofac;
using LiteTube.StreamVideo.Web;
using LiteTube.StreamVideo.Web.ClientReader;
using LiteTube.StreamVideo.Web.Platform;

namespace LiteTube.StreamVideo.Module
{
    public class HttpClientModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClientWebReaderManager>().As<IWebReaderManager>().SingleInstance();
            builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>().SingleInstance();
            builder.RegisterType<HttpClientFactoryParameters>().As<IHttpClientFactoryParameters>().SingleInstance();
            builder.RegisterType<ProductInfoHeaderValueFactory>().As<IProductInfoHeaderValueFactory>().SingleInstance();
            builder.RegisterType<HttpClientHandler>().AsSelf().ExternallyOwned();
        }
    }
}
