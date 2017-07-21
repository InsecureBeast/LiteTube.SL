using System.Collections.Generic;
using SM.Media.Core.Configuration;

namespace SM.Media.Core.Metadata
{
  public class ConfigurationMetadata : IConfigurationMetadata
  {
    public IAudioConfigurationSource Audio { get; set; }

    public IVideoConfigurationSource Video { get; set; }

    public IReadOnlyCollection<IConfigurationSource> AlternateStreams { get; set; }
  }
}
