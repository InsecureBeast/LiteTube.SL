// Decompiled with JetBrains decompiler
// Type: SM.Media.Configuration.VideoConfigurator
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Metadata;

namespace SM.Media.Configuration
{
  public abstract class VideoConfigurator : ConfiguratorBase, IVideoConfigurationSource, IConfigurationSource
  {
    public int? Height { get; protected set; }

    public int? Width { get; protected set; }

    public int? FrameRateNumerator { get; protected set; }

    public int? FrameRateDenominator { get; protected set; }

    public string VideoFourCc { get; protected set; }

    protected VideoConfigurator(string fourCc, ContentType contentType, IMediaStreamMetadata mediaStreamMetadata)
      : base(contentType, mediaStreamMetadata)
    {
      this.VideoFourCc = fourCc;
    }
  }
}
