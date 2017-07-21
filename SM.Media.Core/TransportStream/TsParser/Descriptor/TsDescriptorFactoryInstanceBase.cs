using System;

namespace SM.Media.Core.TransportStream.TsParser.Descriptor
{
  public abstract class TsDescriptorFactoryInstanceBase : ITsDescriptorFactoryInstance
  {
    private readonly TsDescriptorType _type;

    public TsDescriptorType Type
    {
      get
      {
        return this._type;
      }
    }

    protected TsDescriptorFactoryInstanceBase(TsDescriptorType type)
    {
      if (null == type)
        throw new ArgumentNullException("type");
      this._type = type;
    }

    public abstract TsDescriptor Create(byte[] buffer, int offset, int length);
  }
}
