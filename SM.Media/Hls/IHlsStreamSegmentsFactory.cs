// Decompiled with JetBrains decompiler
// Type: SM.Media.Hls.IHlsStreamSegmentsFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8;
using SM.Media.Web;

namespace SM.Media.Hls
{
  public interface IHlsStreamSegmentsFactory
  {
    IHlsStreamSegments Create(M3U8Parser parser, IWebReader webReader);
  }
}
