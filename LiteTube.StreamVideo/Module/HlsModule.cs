using Autofac;
using LiteTube.StreamVideo.Hls;
using LiteTube.StreamVideo.Segments;

namespace LiteTube.StreamVideo.Module
{
    public class HlsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HlsPlaylistSegmentManagerFactory>().As<ISegmentManagerFactoryInstance>().SingleInstance().PreserveExistingDefaults();
            builder.RegisterType<HlsProgramManager>().AsSelf().ExternallyOwned();
            builder.RegisterType<HlsProgramStreamFactory>().As<IHlsProgramStreamFactory>().SingleInstance();
            builder.RegisterType<HlsSegmentsFactory>().As<IHlsSegmentsFactory>().ExternallyOwned();
            builder.RegisterType<HlsStreamSegments>().As<IHlsStreamSegments>().ExternallyOwned();
            builder.RegisterType<HlsStreamSegmentsFactory>().As<IHlsStreamSegmentsFactory>().SingleInstance();
            builder.RegisterType<HlsPlaylistSegmentManagerPolicy>().As<IHlsPlaylistSegmentManagerPolicy>().SingleInstance();
        }
    }
}
