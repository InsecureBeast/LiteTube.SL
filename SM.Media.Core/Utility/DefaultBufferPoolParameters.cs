namespace SM.Media.Core.Utility
{
  public class DefaultBufferPoolParameters : IBufferPoolParameters
  {
    public int BaseSize { get; set; }

    public int Pools { get; set; }

    public DefaultBufferPoolParameters()
    {
      this.BaseSize = 327680;
      this.Pools = 2;
    }
  }
}
