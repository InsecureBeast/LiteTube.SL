// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.Descriptor.TsDescriptorFactoryInstanceBase
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.TransportStream.TsParser.Descriptor
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
