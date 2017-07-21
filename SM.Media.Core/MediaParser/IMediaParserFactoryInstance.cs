using SM.Media.Core.Content;

namespace SM.Media.Core.MediaParser
{
  public interface IMediaParserFactoryInstance : IContentServiceFactoryInstance<IMediaParser, IMediaParserParameters>
  {
  }
}
