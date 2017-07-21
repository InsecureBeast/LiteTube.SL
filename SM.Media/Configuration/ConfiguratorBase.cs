// Decompiled with JetBrains decompiler
// Type: SM.Media.Configuration.ConfiguratorBase
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Metadata;
using System;
using System.Threading;

namespace SM.Media.Configuration
{
  public class ConfiguratorBase : IConfigurationSource
  {
    private readonly ContentType _contentType;
    private readonly IMediaStreamMetadata _mediaStreamMetadata;
    private int _isConfigured;

    public virtual string CodecPrivateData { get; protected set; }

    public string Name { get; protected set; }

    public string StreamDescription { get; protected set; }

    public int? Bitrate { get; protected set; }

    public bool IsConfigured
    {
      get
      {
        return 0 != this._isConfigured;
      }
    }

    public ContentType ContentType
    {
      get
      {
        return this._contentType;
      }
    }

    public IMediaStreamMetadata MediaStreamMetadata
    {
      get
      {
        return this._mediaStreamMetadata;
      }
    }

    public event EventHandler ConfigurationComplete;

    protected ConfiguratorBase(ContentType contentType, IMediaStreamMetadata mediaStreamMetadata)
    {
      if ((ContentType) null == contentType)
        throw new ArgumentNullException("contentType");
      this._contentType = contentType;
      this._mediaStreamMetadata = mediaStreamMetadata;
    }

    protected void SetConfigured()
    {
      Interlocked.Exchange(ref this._isConfigured, 1);
      EventHandler eventHandler = this.ConfigurationComplete;
      if (null == eventHandler)
        return;
      this.ConfigurationComplete = (EventHandler) null;
      eventHandler((object) this, EventArgs.Empty);
    }
  }
}
