// Decompiled with JetBrains decompiler
// Type: SM.Media.H262.H262Configurator
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Configuration;
using SM.Media.Content;
using SM.Media.Metadata;
using System.Diagnostics;

namespace SM.Media.H262
{
  public sealed class H262Configurator : VideoConfigurator, IFrameParser
  {
    public int FrameLength
    {
      get
      {
        return 0;
      }
    }

    public H262Configurator(IMediaStreamMetadata mediaStreamMetadata, string streamDescription = null)
      : base("MP2V", ContentTypes.H262, mediaStreamMetadata)
    {
      this.StreamDescription = streamDescription;
    }

    public bool Parse(byte[] buffer, int index, int length)
    {
      this.Configure();
      return true;
    }

    public void Configure()
    {
      this.Name = "H.262";
      Debug.WriteLine("Configuration " + this.Name);
      this.SetConfigured();
    }
  }
}
