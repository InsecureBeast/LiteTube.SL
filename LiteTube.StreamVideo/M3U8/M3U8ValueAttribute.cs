using System;
using LiteTube.StreamVideo.M3U8.AttributeSupport;

namespace LiteTube.StreamVideo.M3U8
{
  public class M3U8ValueAttribute<TValue> : M3U8Attribute
  {
    public M3U8ValueAttribute(string name, bool isRequired, Func<M3U8Attribute, string, M3U8AttributeValueInstance<TValue>> createInstance)
      : base(name, isRequired, (Func<M3U8Attribute, string, M3U8AttributeInstance>) ((attribute, value) => (M3U8AttributeInstance) createInstance(attribute, value)))
    {
    }
  }
}
