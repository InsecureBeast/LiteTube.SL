using System.Collections.Generic;
using SM.Media.Core.Configuration;

namespace SM.Media.Core.Metadata
{
  public interface IConfigurationMetadata
  {
    IAudioConfigurationSource Audio { get; }

    IVideoConfigurationSource Video { get; }

    IReadOnlyCollection<IConfigurationSource> AlternateStreams { get; }
  }
}
