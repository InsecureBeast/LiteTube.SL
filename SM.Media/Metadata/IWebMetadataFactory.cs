// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.IWebMetadataFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Web;

namespace SM.Media.Metadata
{
  public interface IWebMetadataFactory
  {
    IStreamMetadata CreateStreamMetadata(WebResponse webResponse, ContentType contentType = null);

    ISegmentMetadata CreateSegmentMetadata(WebResponse webResponse, ContentType contentType = null);
  }
}
