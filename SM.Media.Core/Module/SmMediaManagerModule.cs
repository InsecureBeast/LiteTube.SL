using Autofac;
using SM.Media.Core.MediaManager;

namespace SM.Media.Core.Module
{
  public class SmMediaManagerModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      RegistrationExtensions.RegisterType<SmMediaManager>(builder).As<IMediaManager>().InstancePerMatchingLifetimeScope((object) "builder-scope");
    }
  }
}
