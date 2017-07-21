namespace SM.Media.Core.TransportStream.TsParser.Descriptor
{
  public interface ITsDescriptorFactoryInstance
  {
    TsDescriptorType Type { get; }

    TsDescriptor Create(byte[] buffer, int offset, int length);
  }
}
