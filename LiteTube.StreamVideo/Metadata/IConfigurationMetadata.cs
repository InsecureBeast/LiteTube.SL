using System.Collections.Generic;
using LiteTube.StreamVideo.Configuration;

namespace LiteTube.StreamVideo.Metadata
{
  public interface IConfigurationMetadata
  {
    IAudioConfigurationSource Audio { get; }

    IVideoConfigurationSource Video { get; }

    IReadOnlyCollection<IConfigurationSource> AlternateStreams { get; }
  }
}
