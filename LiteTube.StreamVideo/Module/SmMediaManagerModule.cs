using Autofac;
using LiteTube.StreamVideo.MediaManager;

namespace LiteTube.StreamVideo.Module
{
  public class SmMediaManagerModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      RegistrationExtensions.RegisterType<SmMediaManager>(builder).As<IMediaManager>().InstancePerMatchingLifetimeScope((object) "builder-scope");
    }
  }
}
