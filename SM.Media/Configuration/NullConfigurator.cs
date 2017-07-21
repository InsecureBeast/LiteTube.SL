// Decompiled with JetBrains decompiler
// Type: SM.Media.Configuration.NullConfigurator
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Metadata;
using System;

namespace SM.Media.Configuration
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
