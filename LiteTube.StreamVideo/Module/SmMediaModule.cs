using Autofac;
using Autofac.Builder;
using LiteTube.StreamVideo.AAC;
using LiteTube.StreamVideo.Ac3;
using LiteTube.StreamVideo.Audio.Shoutcast;
using LiteTube.StreamVideo.Buffering;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.H262;
using LiteTube.StreamVideo.H264;
using LiteTube.StreamVideo.MediaManager;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.MP3;
using LiteTube.StreamVideo.Pes;
using LiteTube.StreamVideo.Pls;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.TransportStream;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.TransportStream.TsParser.Utility;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Utility.RandomGenerators;
using LiteTube.StreamVideo.Utility.TextEncodings;
using LiteTube.StreamVideo.Web;
using ShoutcastMetadataFilterFactory = LiteTube.StreamVideo.Audio.Shoutcast.ShoutcastMetadataFilterFactory;
using Utf8ShoutcastEncodingSelector = LiteTube.StreamVideo.Audio.Shoutcast.Utf8ShoutcastEncodingSelector;

namespace LiteTube.StreamVideo.Module
{
  public class SmMediaModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      Autofac.RegistrationExtensions.RegisterInstance<ContentTypeDetector>(builder, new ContentTypeDetector(ContentTypes.AllTypes)).As<IContentTypeDetector>();
      Autofac.RegistrationExtensions.RegisterType<SegmentManagerFactoryFinder>(builder).As<ISegmentManagerFactoryFinder>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<SegmentManagerFactory>(builder).As<ISegmentManagerFactory>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<SegmentReaderManagerFactory>(builder).As<ISegmentReaderManagerFactory>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<TsPesPacketPool>(builder).As<ITsPesPacketPool>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<BufferPool>(builder).As<IBufferPool>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<DefaultBufferPoolParameters>(builder).As<IBufferPoolParameters>().SingleInstance();
      Autofac.RegistrationExtensions.PreserveExistingDefaults<SimpleSegmentManagerFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<SimpleSegmentManagerFactory>(builder).As<ISegmentManagerFactoryInstance>().SingleInstance());
      Autofac.RegistrationExtensions.PreserveExistingDefaults<PlsSegmentManagerFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<PlsSegmentManagerFactory>(builder).As<ISegmentManagerFactoryInstance>().SingleInstance());
      Autofac.RegistrationExtensions.RegisterType<MediaParserFactoryFinder>(builder).As<IMediaParserFactoryFinder>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<MediaParserFactory>(builder).As<IMediaParserFactory>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<WebMetadataFactory>(builder).As<IWebMetadataFactory>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<MetadataSink>(builder).As<IMetadataSink>();
      Autofac.RegistrationExtensions.RegisterType<Utf8ShoutcastEncodingSelector>(builder).As<IShoutcastEncodingSelector>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<ShoutcastMetadataFilterFactory>(builder).As<IShoutcastMetadataFilterFactory>().SingleInstance();
      Autofac.RegistrationExtensions.PreserveExistingDefaults<AacMediaParserFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<AacMediaParserFactory>(builder).As<IMediaParserFactoryInstance>().SingleInstance());
      Autofac.RegistrationExtensions.PreserveExistingDefaults<Ac3MediaParserFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<Ac3MediaParserFactory>(builder).As<IMediaParserFactoryInstance>().SingleInstance());
      Autofac.RegistrationExtensions.PreserveExistingDefaults<Mp3MediaParserFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<Mp3MediaParserFactory>(builder).As<IMediaParserFactoryInstance>().SingleInstance());
      Autofac.RegistrationExtensions.PreserveExistingDefaults<TsMediaParserFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<TsMediaParserFactory>(builder).As<IMediaParserFactoryInstance>().SingleInstance());
      Autofac.RegistrationExtensions.AsSelf<AacMediaParser, ConcreteReflectionActivatorData>(Autofac.RegistrationExtensions.RegisterType<AacMediaParser>(builder)).ExternallyOwned();
      Autofac.RegistrationExtensions.AsSelf<Ac3MediaParser, ConcreteReflectionActivatorData>(Autofac.RegistrationExtensions.RegisterType<Ac3MediaParser>(builder)).ExternallyOwned();
      Autofac.RegistrationExtensions.AsSelf<Mp3MediaParser, ConcreteReflectionActivatorData>(Autofac.RegistrationExtensions.RegisterType<Mp3MediaParser>(builder)).ExternallyOwned();
      Autofac.RegistrationExtensions.AsSelf<TsMediaParser, ConcreteReflectionActivatorData>(Autofac.RegistrationExtensions.RegisterType<TsMediaParser>(builder)).ExternallyOwned();
      Autofac.RegistrationExtensions.PreserveExistingDefaults<AacStreamHandlerFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<AacStreamHandlerFactory>(builder).As<IPesStreamFactoryInstance>().InstancePerLifetimeScope());
      Autofac.RegistrationExtensions.PreserveExistingDefaults<Ac3StreamHandlerFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<Ac3StreamHandlerFactory>(builder).As<IPesStreamFactoryInstance>().InstancePerLifetimeScope());
      Autofac.RegistrationExtensions.PreserveExistingDefaults<H262StreamHandlerFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<H262StreamHandlerFactory>(builder).As<IPesStreamFactoryInstance>().InstancePerLifetimeScope());
      Autofac.RegistrationExtensions.PreserveExistingDefaults<H264StreamHandlerFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<H264StreamHandlerFactory>(builder).As<IPesStreamFactoryInstance>().InstancePerLifetimeScope());
      Autofac.RegistrationExtensions.PreserveExistingDefaults<Mp3StreamHandlerFactory, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<Mp3StreamHandlerFactory>(builder).As<IPesStreamFactoryInstance>().InstancePerLifetimeScope());
      Autofac.RegistrationExtensions.RegisterType<PesHandlerFactory>(builder).As<IPesHandlerFactory>().SingleInstance();
      Autofac.RegistrationExtensions.AsSelf<PesStreamParameters, ConcreteReflectionActivatorData>(Autofac.RegistrationExtensions.RegisterType<PesStreamParameters>(builder));
      Autofac.RegistrationExtensions.RegisterType<TsDecoder>(builder).As<ITsDecoder>();
      Autofac.RegistrationExtensions.RegisterType<TsTimestamp>(builder).As<ITsTimestamp>();
      Autofac.RegistrationExtensions.RegisterType<PesHandlers>(builder).As<IPesHandlers>();
      Autofac.RegistrationExtensions.RegisterType<WebReaderManagerParameters>(builder).As<IWebReaderManagerParameters>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<MediaManagerParameters>(builder).As<IMediaManagerParameters>().SingleInstance();
      Autofac.RegistrationExtensions.PreserveExistingDefaults<PlsSegmentManagerPolicy, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<PlsSegmentManagerPolicy>(builder).As<IPlsSegmentManagerPolicy>().SingleInstance());
      Autofac.RegistrationExtensions.RegisterType<DefaultBufferingPolicy>(builder).As<IBufferingPolicy>().InstancePerMatchingLifetimeScope((object) "builder-scope");
      Autofac.RegistrationExtensions.RegisterType<BufferingManager>(builder).As<IBufferingManager>();
      Autofac.RegistrationExtensions.RegisterType<RetryManager>(builder).As<IRetryManager>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<SmEncodings>(builder).As<ISmEncodings>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<UserAgent>(builder).As<IUserAgent>().SingleInstance();
      Autofac.RegistrationExtensions.RegisterType<XorShift1024Star>(builder).As<IRandomGenerator>();
      Autofac.RegistrationExtensions.RegisterType<XorShift1024Star>(builder).As<IRandomGenerator<ulong>>();
    }
  }
}
