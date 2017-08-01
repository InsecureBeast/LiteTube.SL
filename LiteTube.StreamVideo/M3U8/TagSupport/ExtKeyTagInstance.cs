using System.Collections.Generic;
using LiteTube.StreamVideo.M3U8.AttributeSupport;

namespace LiteTube.StreamVideo.M3U8.TagSupport
{
  public sealed class ExtKeyTagInstance : AttributesTagInstance
  {
    internal ExtKeyTagInstance(M3U8Tag tag, IEnumerable<M3U8AttributeInstance> attributes)
      : base(tag, attributes)
    {
    }

    public static ExtKeyTagInstance Create(M3U8Tag tag, string value)
    {
      return new ExtKeyTagInstance(tag, AttributesTagInstance.ParseAttributes(value, ExtKeySupport.Attributes));
    }
  }
}
