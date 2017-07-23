using Autofac;
using SM.Media.Core.Hls;
using SM.Media.Core.Segments;

namespace SM.Media.Core.Module
{
    public class HlsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegistrationExtensions.PreserveExistingDefaults(RegistrationExtensions.RegisterType<HlsPlaylistSegmentManagerFactory>(builder).As<ISegmentManagerFactoryInstance>().SingleInstance());
            RegistrationExtensions.AsSelf(RegistrationExtensions.RegisterType<HlsProgramManager>(builder)).ExternallyOwned();
            RegistrationExtensions.RegisterType<HlsProgramStreamFactory>(builder).As<IHlsProgramStreamFactory>().SingleInstance();
            RegistrationExtensions.RegisterType<HlsSegmentsFactory>(builder).As<IHlsSegmentsFactory>().ExternallyOwned();
            RegistrationExtensions.RegisterType<HlsStreamSegments>(builder).As<IHlsStreamSegments>().ExternallyOwned();
            RegistrationExtensions.RegisterType<HlsStreamSegmentsFactory>(builder).As<IHlsStreamSegmentsFactory>().SingleInstance();
            RegistrationExtensions.RegisterType<HlsPlaylistSegmentManagerPolicy>(builder).As<IHlsPlaylistSegmentManagerPolicy>().SingleInstance();
        }
    }
}
