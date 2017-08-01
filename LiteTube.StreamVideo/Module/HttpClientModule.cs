using System.Net.Http;
using Autofac;
using Autofac.Builder;
using LiteTube.StreamVideo.Web;
using LiteTube.StreamVideo.Web.ClientReader;
using LiteTube.StreamVideo.Web.Platform;

namespace LiteTube.StreamVideo.Module
{
    public class HttpClientModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Autofac.RegistrationExtensions.RegisterType<HttpClientWebReaderManager>(builder).As<IWebReaderManager>().SingleInstance();
            Autofac.RegistrationExtensions.RegisterType<HttpClientFactory>(builder).As<IHttpClientFactory>().SingleInstance();
            Autofac.RegistrationExtensions.RegisterType<HttpClientFactoryParameters>(builder).As<IHttpClientFactoryParameters>().SingleInstance();
            Autofac.RegistrationExtensions.RegisterType<ProductInfoHeaderValueFactory>(builder).As<IProductInfoHeaderValueFactory>().SingleInstance();
            Autofac.RegistrationExtensions.AsSelf<HttpClientHandler, ConcreteReflectionActivatorData>(Autofac.RegistrationExtensions.RegisterType<HttpClientHandler>(builder)).ExternallyOwned();
        }
    }
}
