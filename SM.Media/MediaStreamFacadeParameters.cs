// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaStreamFacadeParameters
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media
{
  public class MediaStreamFacadeParameters
  {
    public static TimeSpan DefaultStartTimeout = TimeSpan.FromSeconds(10.0);

    public Func<IMediaStreamFacadeBase> Factory { get; set; }

    public bool UseHttpConnection { get; set; }

    public bool UseSingleStreamMediaManager { get; set; }

    public TimeSpan CreateTimeout { get; set; }

    public MediaStreamFacadeParameters()
    {
      this.CreateTimeout = MediaStreamFacadeParameters.DefaultStartTimeout;
    }
  }
}
