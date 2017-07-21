// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.HlsProgramStreamFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;
using SM.Media.Utility;
using SM.Media.Web;
using System;
using System.Collections.Generic;

namespace SM.Media.Hls
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
