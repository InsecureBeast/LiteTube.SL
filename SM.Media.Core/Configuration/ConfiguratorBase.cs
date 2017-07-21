using System;
using System.Threading;
using SM.Media.Core.Content;
using SM.Media.Core.Metadata;

namespace SM.Media.Core.Configuration
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
