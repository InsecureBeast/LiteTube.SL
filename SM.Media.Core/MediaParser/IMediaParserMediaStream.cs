namespace SM.Media.Core.MediaParser
{
  public interface IMediaParserMediaStream
  {
    IConfigurationSource ConfigurationSource { get; }

    IStreamSource StreamSource { get; }
  }
}
