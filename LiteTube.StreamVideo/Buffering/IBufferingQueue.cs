namespace LiteTube.StreamVideo.Buffering
{
  public interface IBufferingQueue
  {
    void UpdateBufferStatus(BufferStatus bufferStatus);

    void Flush();
  }
}
