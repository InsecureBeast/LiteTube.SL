using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteTube.StreamVideo.TransportStream.TsParser.Descriptor
{
  public class TsDescriptorFactory : ITsDescriptorFactory
  {
    private readonly ITsDescriptorFactoryInstance[] _factories;

    public TsDescriptorFactory(IEnumerable<ITsDescriptorFactoryInstance> factories)
    {
      ITsDescriptorFactoryInstance[] descriptorFactoryInstanceArray = Enumerable.ToArray<ITsDescriptorFactoryInstance>((IEnumerable<ITsDescriptorFactoryInstance>) Enumerable.OrderBy<ITsDescriptorFactoryInstance, byte>(factories, (Func<ITsDescriptorFactoryInstance, byte>) (f => f.Type.Code)));
      this._factories = new ITsDescriptorFactoryInstance[(int) Enumerable.Max<ITsDescriptorFactoryInstance, byte>((IEnumerable<ITsDescriptorFactoryInstance>) descriptorFactoryInstanceArray, (Func<ITsDescriptorFactoryInstance, byte>) (f => f.Type.Code)) + 1];
      foreach (ITsDescriptorFactoryInstance descriptorFactoryInstance in descriptorFactoryInstanceArray)
        this._factories[(int) descriptorFactoryInstance.Type.Code] = descriptorFactoryInstance;
    }

    public TsDescriptor Create(byte code, byte[] buffer, int offset, int length)
    {
      if ((int) code >= this._factories.Length)
        return (TsDescriptor) null;
      ITsDescriptorFactoryInstance descriptorFactoryInstance = this._factories[(int) code];
      if (null == descriptorFactoryInstance)
        return (TsDescriptor) null;
      return descriptorFactoryInstance.Create(buffer, offset, length);
    }
  }
}
