using Autofac;
using Autofac.Builder;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Platform;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Module
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
