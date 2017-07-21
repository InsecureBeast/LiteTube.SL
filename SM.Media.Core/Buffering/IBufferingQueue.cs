namespace SM.Media.Core.Buffering
{
  public interface IBufferingQueue
  {
    void UpdateBufferStatus(BufferStatus bufferStatus);

    void Flush();
  }
}
