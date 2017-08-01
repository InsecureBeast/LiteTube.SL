using System;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.Configuration
{
  public interface IConfigurationSource
  {
    string CodecPrivateData { get; }

    string Name { get; }

    string StreamDescription { get; }

    int? Bitrate { get; }

    ContentType ContentType { get; }

    IMediaStreamMetadata MediaStreamMetadata { get; }

    bool IsConfigured { get; }

    event EventHandler ConfigurationComplete;
  }
}
