namespace LiteTube.StreamVideo.H264
{
  public interface INalParser
  {
    bool Parse(byte[] buffer, int offset, int length, bool hasEscape);
  }
}
