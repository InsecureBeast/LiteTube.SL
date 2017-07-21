// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.M3U8Attribute
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.M3U8
{
  public class M3U8Attribute : IEquatable<M3U8Attribute>
  {
    public readonly Func<M3U8Attribute, string, M3U8AttributeInstance> CreateInstance;
    public readonly bool IsRequired;
    public readonly string Name;

    public M3U8Attribute(string name, bool isRequired, Func<M3U8Attribute, string, M3U8AttributeInstance> createInstance)
    {
      this.Name = name;
      this.IsRequired = isRequired;
      this.CreateInstance = createInstance;
    }

    public static bool operator ==(M3U8Attribute x, M3U8Attribute y)
    {
      if (object.ReferenceEquals((object) x, (object) y))
        return true;
      if ((M3U8Attribute) null == x || (M3U8Attribute) null == y)
        return false;
      return x.Equals(y);
    }

    public static bool operator !=(M3U8Attribute x, M3U8Attribute y)
    {
      return !(x == y);
    }

    public bool Equals(M3U8Attribute other)
    {
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if ((M3U8Attribute) null == other)
        return false;
      return this.Name == other.Name;
    }

    public override bool Equals(object obj)
    {
      M3U8Attribute other = obj as M3U8Attribute;
      if ((M3U8Attribute) null == other)
        return false;
      return this.Equals(other);
    }

    public override int GetHashCode()
    {
      return this.Name.GetHashCode();
    }

    public override string ToString()
    {
      return this.Name;
    }
  }
}
