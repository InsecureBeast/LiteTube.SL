using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Metadata
{
  public interface IWebMetadataFactory
  {
    IStreamMetadata CreateStreamMetadata(WebResponse webResponse, ContentType contentType = null);

    ISegmentMetadata CreateSegmentMetadata(WebResponse webResponse, ContentType contentType = null);
  }
}
