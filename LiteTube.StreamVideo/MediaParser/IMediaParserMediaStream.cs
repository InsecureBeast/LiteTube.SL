namespace LiteTube.StreamVideo.MediaParser
{
  public interface IMediaParserMediaStream
  {
    IConfigurationSource ConfigurationSource { get; }

    IStreamSource StreamSource { get; }
  }
}
