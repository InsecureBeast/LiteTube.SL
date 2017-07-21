using SM.Media.Core.Content;
using SM.Media.Core.Web;

namespace SM.Media.Core.Metadata
{
  public class WebMetadataFactory : IWebMetadataFactory
  {
    public IStreamMetadata CreateStreamMetadata(WebResponse webResponse, ContentType contentType = null)
    {
      ShoutcastHeaders shoutcastHeaders = new ShoutcastHeaders(webResponse.RequestUri, webResponse.Headers);
      return (IStreamMetadata) new StreamMetadata()
      {
        Url = webResponse.RequestUri,
        ContentType = (contentType ?? webResponse.ContentType),
        Bitrate = shoutcastHeaders.Bitrate,
        Description = shoutcastHeaders.Description,
        Genre = shoutcastHeaders.Genre,
        Name = shoutcastHeaders.Name,
        Website = shoutcastHeaders.Website
      };
    }

    public ISegmentMetadata CreateSegmentMetadata(WebResponse webResponse, ContentType contentType)
    {
      ShoutcastHeaders shoutcastHeaders = new ShoutcastHeaders(webResponse.RequestUri, webResponse.Headers);
      int? metaInterval = shoutcastHeaders.MetaInterval;
      if ((metaInterval.GetValueOrDefault() <= 0 ? 0 : (metaInterval.HasValue ? 1 : 0)) != 0 || shoutcastHeaders.SupportsIcyMetadata)
      {
        ShoutcastSegmentMetadata shoutcastSegmentMetadata = new ShoutcastSegmentMetadata();
        shoutcastSegmentMetadata.Url = webResponse.RequestUri;
        shoutcastSegmentMetadata.ContentType = contentType ?? webResponse.ContentType;
        shoutcastSegmentMetadata.Length = webResponse.ContentLength;
        shoutcastSegmentMetadata.IcyMetaInt = shoutcastHeaders.MetaInterval;
        shoutcastSegmentMetadata.SupportsIcyMetadata = shoutcastHeaders.SupportsIcyMetadata;
        return (ISegmentMetadata) shoutcastSegmentMetadata;
      }
      return (ISegmentMetadata) new SegmentMetadata()
      {
        Url = webResponse.RequestUri,
        ContentType = (contentType ?? webResponse.ContentType),
        Length = webResponse.ContentLength
      };
    }
  }
}
