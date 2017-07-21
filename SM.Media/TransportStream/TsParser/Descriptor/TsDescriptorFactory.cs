// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.Descriptor.TsDescriptorFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Media.TransportStream.TsParser.Descriptor
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
