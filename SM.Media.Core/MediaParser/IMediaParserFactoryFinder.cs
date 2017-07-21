using SM.Media.Core.Content;

namespace SM.Media.Core.MediaParser
{
  public interface IMediaParserFactoryFinder : IContentServiceFactoryFinder<IMediaParser, IMediaParserParameters>
  {
    void Register(ContentType contentType, IMediaParserFactoryInstance factory);
  }
}
