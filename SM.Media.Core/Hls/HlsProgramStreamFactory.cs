using System;
using System.Collections.Generic;
using SM.Media.Core.Metadata;
using SM.Media.Core.Utility;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
{
  public class HlsProgramStreamFactory : IHlsProgramStreamFactory
  {
    private readonly IPlatformServices _platformServices;
    private readonly IRetryManager _retryManager;
    private readonly IHlsSegmentsFactory _segmentsFactory;
    private readonly IWebMetadataFactory _webMetadataFactory;

    public HlsProgramStreamFactory(IHlsSegmentsFactory segmentsFactory, IWebMetadataFactory webMetadataFactory, IPlatformServices platformServices, IRetryManager retryManager)
    {
      if (null == segmentsFactory)
        throw new ArgumentNullException("segmentsFactory");
      if (null == webMetadataFactory)
        throw new ArgumentNullException("webMetadataFactory");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      this._segmentsFactory = segmentsFactory;
      this._webMetadataFactory = webMetadataFactory;
      this._platformServices = platformServices;
      this._retryManager = retryManager;
    }

    public IHlsProgramStream Create(ICollection<Uri> urls, IWebReader webReader)
    {
      return (IHlsProgramStream) new HlsProgramStream(webReader, urls, this._segmentsFactory, this._webMetadataFactory, this._platformServices, this._retryManager);
    }
  }
}
