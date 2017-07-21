namespace SM.Media.Core.Buffering
{
  public interface IQueueThrottling
  {
    void Pause();

    void Resume();
  }
}
