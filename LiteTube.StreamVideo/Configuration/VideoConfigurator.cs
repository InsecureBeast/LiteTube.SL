using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.Configuration
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
