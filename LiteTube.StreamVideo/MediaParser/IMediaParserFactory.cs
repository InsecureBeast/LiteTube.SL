using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.MediaParser
{
  public interface IMediaParserFactory : IContentServiceFactory<IMediaParser, IMediaParserParameters>
  {
  }
}
