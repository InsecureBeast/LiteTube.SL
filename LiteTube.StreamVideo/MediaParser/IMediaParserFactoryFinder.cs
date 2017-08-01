using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.MediaParser
{
  public interface IMediaParserFactoryFinder : IContentServiceFactoryFinder<IMediaParser, IMediaParserParameters>
  {
    void Register(ContentType contentType, IMediaParserFactoryInstance factory);
  }
}
