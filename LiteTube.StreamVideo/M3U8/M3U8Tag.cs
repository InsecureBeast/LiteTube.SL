using System;

namespace LiteTube.StreamVideo.M3U8
{
  public class M3U8Tag : IEquatable<M3U8Tag>
  {
    public readonly Func<M3U8Tag, string, M3U8TagInstance> CreateInstance;
    public readonly string Name;
    public readonly M3U8TagScope Scope;

    public M3U8Tag(string name, M3U8TagScope scope, Func<M3U8Tag, string, M3U8TagInstance> createInstance)
    {
      this.Name = name;
      this.Scope = scope;
      this.CreateInstance = createInstance;
    }

    public static bool operator ==(M3U8Tag x, M3U8Tag y)
    {
      if (object.ReferenceEquals((object) x, (object) y))
        return true;
      if ((M3U8Tag) null == x || (M3U8Tag) null == y)
        return false;
      return x.Equals(y);
    }

    public static bool operator !=(M3U8Tag x, M3U8Tag y)
    {
      return !(x == y);
    }

    public bool Equals(M3U8Tag other)
    {
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if ((M3U8Tag) null == other)
        return false;
      return this.Name == other.Name;
    }

    public override bool Equals(object obj)
    {
      M3U8Tag other = obj as M3U8Tag;
      if ((M3U8Tag) null == other)
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
