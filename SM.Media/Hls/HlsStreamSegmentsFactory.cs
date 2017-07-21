// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.HlsStreamSegmentsFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using SM.Media.Utility;
using SM.Media.Web;

namespace SM.Media.Hls
{
  public class HlsStreamSegmentsFactory : IHlsStreamSegmentsFactory
  {
    private readonly IPlatformServices _platformServices;
    private readonly IRetryManager _retryManager;

    public HlsStreamSegmentsFactory(IRetryManager retryManager, IPlatformServices platformServices)
    {
      this._retryManager = retryManager;
      this._platformServices = platformServices;
    }

    public IHlsStreamSegments Create(M3U8Parser parser, IWebReader webReader)
    {
      return (IHlsStreamSegments) new HlsStreamSegments(parser, webReader, this._retryManager, this._platformServices);
    }
  }
}
