using Autofac;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Platform;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Module
{
    public class TsMediaModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MediaStreamConfigurator>().As<IMediaStreamConfigurator>().As<IMediaStreamControl>().InstancePerMatchingLifetimeScope("builder-scope");
            builder.RegisterType<TsMediaStreamSource>().AsSelf().ExternallyOwned();
            builder.RegisterType<PlatformServices>().As<IPlatformServices>().SingleInstance();
        }
    }
}
