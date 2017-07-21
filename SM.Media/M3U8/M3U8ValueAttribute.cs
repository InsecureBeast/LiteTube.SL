// Decompiled with JetBrains decompiler
// Type: SM.Media.M3U8.M3U8ValueAttribute`1
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.M3U8.AttributeSupport;
using System;

namespace SM.Media.M3U8
{
  public class M3U8ValueAttribute<TValue> : M3U8Attribute
  {
    public M3U8ValueAttribute(string name, bool isRequired, Func<M3U8Attribute, string, M3U8AttributeValueInstance<TValue>> createInstance)
      : base(name, isRequired, (Func<M3U8Attribute, string, M3U8AttributeInstance>) ((attribute, value) => (M3U8AttributeInstance) createInstance(attribute, value)))
    {
    }
  }
}
