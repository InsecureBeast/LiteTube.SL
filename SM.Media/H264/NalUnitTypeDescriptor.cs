// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.NalUnitTypeDescriptor
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.H264
{
  public sealed class NalUnitTypeDescriptor : IEquatable<NalUnitTypeDescriptor>
  {
    private readonly string _description;
    private readonly string _name;
    private readonly NalUnitType _type;

    public NalUnitType Type
    {
      get
      {
        return this._type;
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public string Description
    {
      get
      {
        return this._description;
      }
    }

    public NalUnitTypeDescriptor(NalUnitType type, string name, string description)
    {
      this._type = type;
      this._name = name;
      this._description = description;
    }

    public static bool operator ==(NalUnitTypeDescriptor a, NalUnitTypeDescriptor b)
    {
      if (object.ReferenceEquals((object) a, (object) b))
        return true;
      if (object.ReferenceEquals((object) a, (object) null))
        return false;
      return a.Equals(b);
    }

    public static bool operator !=(NalUnitTypeDescriptor a, NalUnitTypeDescriptor b)
    {
      return !(a == b);
    }

    public bool Equals(NalUnitTypeDescriptor other)
    {
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      return this._type == other._type;
    }

    public override int GetHashCode()
    {
      return (int) this._type;
    }

    public override bool Equals(object obj)
    {
      NalUnitTypeDescriptor other = obj as NalUnitTypeDescriptor;
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      return this.Equals(other);
    }

    public override string ToString()
    {
      return string.Format("{0}/{1}", new object[2]
      {
        (object) this._type,
        (object) this._name
      });
    }
  }
}
