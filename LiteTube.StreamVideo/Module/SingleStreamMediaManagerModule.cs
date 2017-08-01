using Autofac;
using LiteTube.StreamVideo.MediaManager;

namespace LiteTube.StreamVideo.Module
{
  public class SingleStreamMediaManagerModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<SingleStreamMediaManager>().As<IMediaManager>().InstancePerMatchingLifetimeScope("builder-scope");
    }
  }
}
