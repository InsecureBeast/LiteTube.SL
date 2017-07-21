// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.Descriptor.TsDescriptorType
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.TransportStream.TsParser.Descriptor
{
  public class TsDescriptorType : IEquatable<TsDescriptorType>
  {
    private readonly byte _code;
    private readonly string _description;

    public byte Code
    {
      get
      {
        return this._code;
      }
    }

    public string Description
    {
      get
      {
        return this._description;
      }
    }

    public TsDescriptorType(byte code, string description)
    {
      this._code = code;
      this._description = description;
    }

    public bool Equals(TsDescriptorType other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      return (int) this._code == (int) other._code;
    }

    public override int GetHashCode()
    {
      return this._code.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as TsDescriptorType);
    }

    public override string ToString()
    {
      return (string) (object) this._code + (object) ":" + this._description;
    }
  }
}
