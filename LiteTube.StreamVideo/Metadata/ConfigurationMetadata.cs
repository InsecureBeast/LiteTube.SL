using System.Collections.Generic;
using LiteTube.StreamVideo.Configuration;

namespace LiteTube.StreamVideo.Metadata
{
  public class ConfigurationMetadata : IConfigurationMetadata
  {
    public IAudioConfigurationSource Audio { get; set; }

    public IVideoConfigurationSource Video { get; set; }

    public IReadOnlyCollection<IConfigurationSource> AlternateStreams { get; set; }
  }
}
