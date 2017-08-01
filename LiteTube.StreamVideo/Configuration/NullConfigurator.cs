using System;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.Configuration
{
  public sealed class NullConfigurator : IConfigurationSource
  {
    public string CodecPrivateData
    {
      get
      {
        return (string) null;
      }
    }

    public string Name
    {
      get
      {
        return "Unknown";
      }
    }

    public string StreamDescription
    {
      get
      {
        return (string) null;
      }
    }

    public int? Bitrate
    {
      get
      {
        return new int?();
      }
    }

    public ContentType ContentType
    {
      get
      {
        return ContentTypes.Binary;
      }
    }

    public IMediaStreamMetadata MediaStreamMetadata
    {
      get
      {
        return (IMediaStreamMetadata) null;
      }
    }

    public bool IsConfigured
    {
      get
      {
        return true;
      }
    }

    public event EventHandler ConfigurationComplete;
  }
}
