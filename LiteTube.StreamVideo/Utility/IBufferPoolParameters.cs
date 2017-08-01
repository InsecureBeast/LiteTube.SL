namespace LiteTube.StreamVideo.Utility
{
  public interface IBufferPoolParameters
  {
    int BaseSize { get; set; }

    int Pools { get; set; }
  }
}
