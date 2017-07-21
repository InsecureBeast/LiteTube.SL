using Autofac;
using Autofac.Builder;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Descriptor;

namespace SM.Media.Core.Module
{
  public class TsParserModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      Autofac.RegistrationExtensions.RegisterType<TsDecoder>(builder).As<ITsDecoder>().ExternallyOwned();
      Autofac.RegistrationExtensions.RegisterType<TsProgramAssociationTableFactory>(builder).As<ITsProgramAssociationTableFactory>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<TsProgramMapTableFactory>(builder).As<ITsProgramMapTableFactory>().SingleInstance();
      Autofac.RegistrationExtensions.PreserveExistingDefaults<TsIso639LanguageDescriptorFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<TsIso639LanguageDescriptorFactory>(builder).As<ITsDescriptorFactoryInstance>().SingleInstance());
      Autofac.RegistrationExtensions.RegisterType<TsDescriptorFactory>(builder).As<ITsDescriptorFactory>().SingleInstance();
    }
  }
}
