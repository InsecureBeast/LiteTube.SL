using Autofac;
using SM.Media.Core.MediaManager;

namespace SM.Media.Core.Module
{
  public class SingleStreamMediaManagerModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      RegistrationExtensions.RegisterType<SingleStreamMediaManager>(builder).As<IMediaManager>().InstancePerMatchingLifetimeScope((object) "builder-scope");
    }
  }
}
