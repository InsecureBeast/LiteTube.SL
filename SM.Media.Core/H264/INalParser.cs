namespace SM.Media.Core.H264
{
  public interface INalParser
  {
    bool Parse(byte[] buffer, int offset, int length, bool hasEscape);
  }
}
