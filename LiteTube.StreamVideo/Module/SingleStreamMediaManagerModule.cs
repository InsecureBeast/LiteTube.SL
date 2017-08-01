using Autofac;
using LiteTube.StreamVideo.MediaManager;

namespace LiteTube.StreamVideo.Module
{
  public class SingleStreamMediaManagerModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      RegistrationExtensions.RegisterType<SingleStreamMediaManager>(builder).As<IMediaManager>().InstancePerMatchingLifetimeScope((object) "builder-scope");
    }
  }
}
