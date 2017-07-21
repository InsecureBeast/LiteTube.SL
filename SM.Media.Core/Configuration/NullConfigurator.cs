using System;
using SM.Media.Core.Content;
using SM.Media.Core.Metadata;

namespace SM.Media.Core.Configuration
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
