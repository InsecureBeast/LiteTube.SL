namespace SM.Media.Core.Configuration
{
  public interface IFrameParser
  {
    int FrameLength { get; }

    bool Parse(byte[] buffer, int index, int length);
  }
}
