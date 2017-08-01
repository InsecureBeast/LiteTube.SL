using Autofac;
using Autofac.Core;

namespace LiteTube.StreamVideo.Builder
{
  public static class BuilderBaseExtensions
  {
    public static void RegisterModule(this BuilderBase builder, IModule module)
    {
      ModuleRegistrationExtensions.RegisterModule(builder.ContainerBuilder, module);
    }

    public static void RegisterModule<TModule>(this BuilderBase builder) where TModule : IModule, new()
    {
      ModuleRegistrationExtensions.RegisterModule<TModule>(builder.ContainerBuilder);
    }
  }
}
