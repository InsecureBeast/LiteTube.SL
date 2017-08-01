using Autofac;
using LiteTube.StreamVideo.MediaManager;

namespace LiteTube.StreamVideo.Module
{
  public class SmMediaManagerModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<SmMediaManager>().As<IMediaManager>().InstancePerMatchingLifetimeScope("builder-scope");
    }
  }
}
