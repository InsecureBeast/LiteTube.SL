namespace LiteTube.StreamVideo.Configuration
{
  public interface IAudioConfigurationSource : IConfigurationSource
  {
    AudioFormat Format { get; }

    int SamplingFrequency { get; }

    int Channels { get; }
  }
}
