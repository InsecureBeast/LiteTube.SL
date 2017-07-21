using Autofac;
using Autofac.Builder;
using SM.Media.Core.Hls;
using SM.Media.Core.Segments;

namespace SM.Media.Core.Module
{
  public class HlsModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      Autofac.RegistrationExtensions.PreserveExistingDefaults<HlsPlaylistSegmentManagerFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<HlsPlaylistSegmentManagerFactory>(builder).As<ISegmentManagerFactoryInstance>().SingleInstance());
      Autofac.RegistrationExtensions.AsSelf<HlsProgramManager, ConcreteReflectionActivatorData>(Autofac.RegistrationExtensions.RegisterType<HlsProgramManager>(builder)).ExternallyOwned();
      Autofac.RegistrationExtensions.RegisterType<HlsProgramStreamFactory>(builder).As<IHlsProgramStreamFactory>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<HlsSegmentsFactory>(builder).As<IHlsSegmentsFactory>().ExternallyOwned();
      Autofac.RegistrationExtensions.RegisterType<HlsStreamSegments>(builder).As<IHlsStreamSegments>().ExternallyOwned();
      Autofac.RegistrationExtensions.RegisterType<HlsStreamSegmentsFactory>(builder).As<IHlsStreamSegmentsFactory>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<HlsPlaylistSegmentManagerPolicy>(builder).As<IHlsPlaylistSegmentManagerPolicy>().SingleInstance();
    }
  }
}
