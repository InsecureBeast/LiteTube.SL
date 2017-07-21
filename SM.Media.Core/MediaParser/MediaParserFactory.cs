using SM.Media.Core.Content;

namespace SM.Media.Core.MediaParser
{
  public class MediaParserFactory : ContentServiceFactory<IMediaParser, IMediaParserParameters>, IMediaParserFactory, IContentServiceFactory<IMediaParser, IMediaParserParameters>
  {
    public MediaParserFactory(IMediaParserFactoryFinder factoryFinder)
      : base((IContentServiceFactoryFinder<IMediaParser, IMediaParserParameters>) factoryFinder)
    {
    }
  }
}
