namespace SM.Media.Core.Configuration
{
  public interface IVideoConfigurationSource : IConfigurationSource
  {
    string VideoFourCc { get; }

    int? Height { get; }

    int? Width { get; }

    int? FrameRateNumerator { get; }

    int? FrameRateDenominator { get; }
  }
}
