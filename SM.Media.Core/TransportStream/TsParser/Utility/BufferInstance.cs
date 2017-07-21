namespace SM.Media.Core.TransportStream.TsParser.Utility
{
  public abstract class BufferInstance
  {
    public readonly byte[] Buffer;

    protected BufferInstance(byte[] buffer)
    {
      this.Buffer = buffer;
    }

    public abstract void Reference();

    public abstract bool Dereference();
  }
}
