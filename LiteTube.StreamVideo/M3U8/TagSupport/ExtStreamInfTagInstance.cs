using System.Collections.Generic;
using LiteTube.StreamVideo.M3U8.AttributeSupport;

namespace LiteTube.StreamVideo.M3U8.TagSupport
{
  public sealed class ExtStreamInfTagInstance : AttributesTagInstance
  {
    internal ExtStreamInfTagInstance(M3U8Tag tag, IEnumerable<M3U8AttributeInstance> attributes)
      : base(tag, attributes)
    {
    }

    public static ExtStreamInfTagInstance Create(M3U8Tag tag, string value)
    {
      return new ExtStreamInfTagInstance(tag, AttributesTagInstance.ParseAttributes(value, ExtStreamInfSupport.Attributes));
    }
  }
}
