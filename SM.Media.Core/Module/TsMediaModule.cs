using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using SM.Media.Core.MediaParser;
using SM.Media.Core.Platform;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Module
{
    public class TsMediaModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Autofac.RegistrationExtensions.RegisterType<MediaStreamConfigurator>(builder).As<IMediaStreamConfigurator>().As<IMediaStreamControl>().InstancePerMatchingLifetimeScope((object)"builder-scope");
            Autofac.RegistrationExtensions.AsSelf<TsMediaStreamSource, ConcreteReflectionActivatorData>(Autofac.RegistrationExtensions.RegisterType<TsMediaStreamSource>(builder)).ExternallyOwned();
            Autofac.RegistrationExtensions.RegisterType<PlatformServices>(builder).As<IPlatformServices>().SingleInstance();
        }
    }
}
