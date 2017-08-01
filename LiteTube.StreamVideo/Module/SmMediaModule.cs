using Autofac;
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
      builder.RegisterInstance(new ContentTypeDetector(ContentTypes.AllTypes)).As<IContentTypeDetector>();
      builder.RegisterType<SegmentManagerFactoryFinder>().As<ISegmentManagerFactoryFinder>().SingleInstance();
      builder.RegisterType<SegmentManagerFactory>().As<ISegmentManagerFactory>().SingleInstance();
      builder.RegisterType<SegmentReaderManagerFactory>().As<ISegmentReaderManagerFactory>().SingleInstance();
      builder.RegisterType<TsPesPacketPool>().As<ITsPesPacketPool>().SingleInstance();
      builder.RegisterType<BufferPool>().As<IBufferPool>().SingleInstance();
      builder.RegisterType<DefaultBufferPoolParameters>().As<IBufferPoolParameters>().SingleInstance();
      builder.RegisterType<SimpleSegmentManagerFactory>().As<ISegmentManagerFactoryInstance>().SingleInstance().PreserveExistingDefaults();
      builder.RegisterType<PlsSegmentManagerFactory>().As<ISegmentManagerFactoryInstance>().SingleInstance().PreserveExistingDefaults();
      builder.RegisterType<MediaParserFactoryFinder>().As<IMediaParserFactoryFinder>().SingleInstance();
      builder.RegisterType<MediaParserFactory>().As<IMediaParserFactory>().SingleInstance();
      builder.RegisterType<WebMetadataFactory>().As<IWebMetadataFactory>().SingleInstance();
      builder.RegisterType<MetadataSink>().As<IMetadataSink>();
      builder.RegisterType<Utf8ShoutcastEncodingSelector>().As<IShoutcastEncodingSelector>().SingleInstance();
      builder.RegisterType<ShoutcastMetadataFilterFactory>().As<IShoutcastMetadataFilterFactory>().SingleInstance();
      builder.RegisterType<AacMediaParserFactory>().As<IMediaParserFactoryInstance>().SingleInstance().PreserveExistingDefaults();
      builder.RegisterType<Ac3MediaParserFactory>().As<IMediaParserFactoryInstance>().SingleInstance().PreserveExistingDefaults();
      builder.RegisterType<Mp3MediaParserFactory>().As<IMediaParserFactoryInstance>().SingleInstance().PreserveExistingDefaults();
      builder.RegisterType<TsMediaParserFactory>().As<IMediaParserFactoryInstance>().SingleInstance().PreserveExistingDefaults();
      builder.RegisterType<AacMediaParser>().AsSelf().ExternallyOwned();
      builder.RegisterType<Ac3MediaParser>().AsSelf().ExternallyOwned();
      builder.RegisterType<Mp3MediaParser>().AsSelf().ExternallyOwned();
      builder.RegisterType<TsMediaParser>().AsSelf().ExternallyOwned();
      builder.RegisterType<AacStreamHandlerFactory>().As<IPesStreamFactoryInstance>().InstancePerLifetimeScope().PreserveExistingDefaults();
      builder.RegisterType<Ac3StreamHandlerFactory>().As<IPesStreamFactoryInstance>().InstancePerLifetimeScope().PreserveExistingDefaults();
      builder.RegisterType<H262StreamHandlerFactory>().As<IPesStreamFactoryInstance>().InstancePerLifetimeScope().PreserveExistingDefaults();
      builder.RegisterType<H264StreamHandlerFactory>().As<IPesStreamFactoryInstance>().InstancePerLifetimeScope().PreserveExistingDefaults();
      builder.RegisterType<Mp3StreamHandlerFactory>().As<IPesStreamFactoryInstance>().InstancePerLifetimeScope().PreserveExistingDefaults();
      builder.RegisterType<PesHandlerFactory>().As<IPesHandlerFactory>().SingleInstance();
      builder.RegisterType<PesStreamParameters>().AsSelf();
      builder.RegisterType<TsDecoder>().As<ITsDecoder>();
      builder.RegisterType<TsTimestamp>().As<ITsTimestamp>();
      builder.RegisterType<PesHandlers>().As<IPesHandlers>();
      builder.RegisterType<WebReaderManagerParameters>().As<IWebReaderManagerParameters>().SingleInstance();
      builder.RegisterType<MediaManagerParameters>().As<IMediaManagerParameters>().SingleInstance();
      builder.RegisterType<PlsSegmentManagerPolicy>().As<IPlsSegmentManagerPolicy>().SingleInstance().PreserveExistingDefaults();
      builder.RegisterType<DefaultBufferingPolicy>().As<IBufferingPolicy>().InstancePerMatchingLifetimeScope("builder-scope");
      builder.RegisterType<BufferingManager>().As<IBufferingManager>();
      builder.RegisterType<RetryManager>().As<IRetryManager>().SingleInstance();
      builder.RegisterType<SmEncodings>().As<ISmEncodings>().SingleInstance();
      builder.RegisterType<UserAgent>().As<IUserAgent>().SingleInstance();
      builder.RegisterType<XorShift1024Star>().As<IRandomGenerator>();
      builder.RegisterType<XorShift1024Star>().As<IRandomGenerator<ulong>>();
    }
  }
}
