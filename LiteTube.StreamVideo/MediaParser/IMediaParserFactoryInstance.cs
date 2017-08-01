using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.MediaParser
{
  public interface IMediaParserFactoryInstance : IContentServiceFactoryInstance<IMediaParser, IMediaParserParameters>
  {
  }
}
