using LiteTube.StreamVideo.Configuration;

namespace LiteTube.StreamVideo.Audio
{
  public interface IAudioConfigurator : IAudioConfigurationSource, IConfigurationSource
  {
    void Configure(IAudioFrameHeader frameHeader);
  }
}
