using Autofac;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Descriptor;

namespace LiteTube.StreamVideo.Module
{
  public class TsParserModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<TsDecoder>().As<ITsDecoder>().ExternallyOwned();
      builder.RegisterType<TsProgramAssociationTableFactory>().As<ITsProgramAssociationTableFactory>().SingleInstance();
      builder.RegisterType<TsProgramMapTableFactory>().As<ITsProgramMapTableFactory>().SingleInstance();
      builder.RegisterType<TsIso639LanguageDescriptorFactory>().As<ITsDescriptorFactoryInstance>().SingleInstance().PreserveExistingDefaults();
      builder.RegisterType<TsDescriptorFactory>().As<ITsDescriptorFactory>().SingleInstance();
    }
  }
}
