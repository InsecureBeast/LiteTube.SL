namespace SM.Media.Core.TransportStream.TsParser.Descriptor
{
  public interface ITsDescriptorFactory
  {
    TsDescriptor Create(byte code, byte[] buffer, int offset, int length);
  }
}
