using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.MediaParser
{
  public class MediaParserFactory : ContentServiceFactory<IMediaParser, IMediaParserParameters>, IMediaParserFactory, IContentServiceFactory<IMediaParser, IMediaParserParameters>
  {
    public MediaParserFactory(IMediaParserFactoryFinder factoryFinder)
      : base((IContentServiceFactoryFinder<IMediaParser, IMediaParserParameters>) factoryFinder)
    {
    }
  }
}
