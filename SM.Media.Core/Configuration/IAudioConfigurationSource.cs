namespace SM.Media.Core.Configuration
{
  public interface IAudioConfigurationSource : IConfigurationSource
  {
    AudioFormat Format { get; }

    int SamplingFrequency { get; }

    int Channels { get; }
  }
}
