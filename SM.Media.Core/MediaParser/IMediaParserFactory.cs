using SM.Media.Core.Content;

namespace SM.Media.Core.MediaParser
{
  public interface IMediaParserFactory : IContentServiceFactory<IMediaParser, IMediaParserParameters>
  {
  }
}
