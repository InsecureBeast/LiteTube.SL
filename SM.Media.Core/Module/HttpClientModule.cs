using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using SM.Media.Core.Web;
using SM.Media.Core.Web.ClientReader;
using SM.Media.Core.Web.Platform;

namespace SM.Media.Core.Module
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
