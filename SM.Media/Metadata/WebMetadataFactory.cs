// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.WebMetadataFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Web;

namespace SM.Media.Metadata
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
