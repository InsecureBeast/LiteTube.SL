namespace LiteTube.StreamVideo.Buffering
{
  public interface IQueueThrottling
  {
    void Pause();

    void Resume();
  }
}
