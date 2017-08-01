using System.Diagnostics;
using LiteTube.StreamVideo.Configuration;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.H262
{
  public sealed class H262Configurator : VideoConfigurator, IFrameParser, IConfigurationSource
  {
    public int FrameLength
    {
      get
      {
        return 0;
      }
    }

    public H262Configurator(IMediaStreamMetadata mediaStreamMetadata, string streamDescription = null)
      : base("MP2V", ContentTypes.H262, mediaStreamMetadata)
    {
      this.StreamDescription = streamDescription;
    }

    public bool Parse(byte[] buffer, int index, int length)
    {
      this.Configure();
      return true;
    }

    public void Configure()
    {
      this.Name = "H.262";
      Debug.WriteLine("Configuration " + this.Name);
      this.SetConfigured();
    }
  }
}
