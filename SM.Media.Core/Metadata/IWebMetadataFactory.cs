using SM.Media.Core.Content;
using SM.Media.Core.Web;

namespace SM.Media.Core.Metadata
{
  public interface IWebMetadataFactory
  {
    IStreamMetadata CreateStreamMetadata(WebResponse webResponse, ContentType contentType = null);

    ISegmentMetadata CreateSegmentMetadata(WebResponse webResponse, ContentType contentType = null);
  }
}
