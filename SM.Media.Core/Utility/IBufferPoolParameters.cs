namespace SM.Media.Core.Utility
{
  public interface IBufferPoolParameters
  {
    int BaseSize { get; set; }

    int Pools { get; set; }
  }
}
