namespace LiteTube.StreamVideo.TransportStream.TsParser.Descriptor
{
  public class TsDescriptor
  {
    private readonly TsDescriptorType _type;

    public TsDescriptorType Type
    {
      get
      {
        return this._type;
      }
    }

    public TsDescriptor(TsDescriptorType type)
    {
      this._type = type;
    }
  }
}
