using SM.Media.Core.Configuration;

namespace SM.Media.Core.Audio
{
  public interface IAudioConfigurator : IAudioConfigurationSource, IConfigurationSource
  {
    void Configure(IAudioFrameHeader frameHeader);
  }
}
