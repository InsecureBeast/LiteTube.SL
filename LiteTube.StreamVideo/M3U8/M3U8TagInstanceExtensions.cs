using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteTube.StreamVideo.M3U8.AttributeSupport;
using LiteTube.StreamVideo.M3U8.TagSupport;

namespace LiteTube.StreamVideo.M3U8
{
  public static class M3U8TagInstanceExtensions
  {
    private static readonly IEnumerable<M3U8AttributeInstance> NoAttributeInstances = (IEnumerable<M3U8AttributeInstance>) new M3U8AttributeInstance[0];

    public static IEnumerable<M3U8AttributeInstance> Attributes(this M3U8TagInstance tagInstance)
    {
      AttributesTagInstance attributesTagInstance = tagInstance as AttributesTagInstance;
      if (null == attributesTagInstance)
        return M3U8TagInstanceExtensions.NoAttributeInstances;
      return attributesTagInstance.Attributes;
    }

    public static IEnumerable<M3U8AttributeInstance> Attributes(this M3U8TagInstance tagInstance, M3U8Attribute attribute)
    {
      return Enumerable.Where<M3U8AttributeInstance>(M3U8TagInstanceExtensions.Attributes(tagInstance), (Func<M3U8AttributeInstance, bool>) (a => a.Attribute == attribute));
    }

    public static TInstance AttributeInstance<TInstance>(this M3U8TagInstance tagInstance, M3U8Attribute attribute) where TInstance : M3U8AttributeInstance
    {
      return Enumerable.FirstOrDefault<M3U8AttributeInstance>(M3U8TagInstanceExtensions.Attributes(tagInstance), (Func<M3U8AttributeInstance, bool>) (a => a.Attribute == attribute)) as TInstance;
    }

    public static IEnumerable<M3U8AttributeValueInstance<TValue>> Attributes<TValue>(this M3U8TagInstance tagInstance, M3U8ValueAttribute<TValue> attribute)
    {
      return Enumerable.Where<M3U8AttributeValueInstance<TValue>>(Enumerable.OfType<M3U8AttributeValueInstance<TValue>>((IEnumerable) M3U8TagInstanceExtensions.Attributes(tagInstance)), (Func<M3U8AttributeValueInstance<TValue>, bool>) (a => a.Attribute == (M3U8Attribute) attribute));
    }

    public static M3U8AttributeValueInstance<TValue> Attribute<TValue>(this M3U8TagInstance tagInstance, M3U8ValueAttribute<TValue> attribute)
    {
      return Enumerable.FirstOrDefault<M3U8AttributeValueInstance<TValue>>(Enumerable.OfType<M3U8AttributeValueInstance<TValue>>((IEnumerable) M3U8TagInstanceExtensions.Attributes(tagInstance)), (Func<M3U8AttributeValueInstance<TValue>, bool>) (a => a.Attribute == (M3U8Attribute) attribute));
    }

    public static M3U8AttributeValueInstance<TValue> Attribute<TValue>(this M3U8TagInstance tagInstance, M3U8ValueAttribute<TValue> attribute, TValue value) where TValue : IEquatable<TValue>
    {
      return Enumerable.FirstOrDefault<M3U8AttributeValueInstance<TValue>>(Enumerable.OfType<M3U8AttributeValueInstance<TValue>>((IEnumerable) M3U8TagInstanceExtensions.Attributes(tagInstance)), (Func<M3U8AttributeValueInstance<TValue>, bool>) (a => a.Attribute == (M3U8Attribute) attribute && a.Value.Equals(value)));
    }

    public static TValue? AttributeValue<TValue>(this M3U8TagInstance tagInstance, M3U8ValueAttribute<TValue> attribute) where TValue : struct
    {
      M3U8AttributeValueInstance<TValue> attributeValueInstance = M3U8TagInstanceExtensions.Attribute<TValue>(tagInstance, attribute);
      if (null == attributeValueInstance)
        return new TValue?();
      return new TValue?(attributeValueInstance.Value);
    }

    public static TValue AttributeObject<TValue>(this M3U8TagInstance tagInstance, M3U8ValueAttribute<TValue> attribute) where TValue : class
    {
      M3U8AttributeValueInstance<TValue> attributeValueInstance = M3U8TagInstanceExtensions.Attribute<TValue>(tagInstance, attribute);
      if (null == attributeValueInstance)
        return default (TValue);
      return attributeValueInstance.Value;
    }

    public static M3U8TagInstance Tag(this IEnumerable<M3U8TagInstance> tags, M3U8Tag tag)
    {
      return Enumerable.FirstOrDefault<M3U8TagInstance>(tags, (Func<M3U8TagInstance, bool>) (t => t.Tag == tag));
    }

    public static TTagInstance Tag<TTag, TTagInstance>(this IEnumerable<M3U8TagInstance> tags, TTag tag) where TTag : M3U8Tag where TTagInstance : M3U8TagInstance
    {
      return Enumerable.FirstOrDefault<TTagInstance>(Enumerable.OfType<TTagInstance>((IEnumerable) tags), (Func<TTagInstance, bool>) (t => t.Tag == (M3U8Tag) tag));
    }

    public static IEnumerable<M3U8TagInstance> Tags(this IEnumerable<M3U8TagInstance> tags, M3U8Tag tag)
    {
      return Enumerable.Where<M3U8TagInstance>(tags, (Func<M3U8TagInstance, bool>) (t => t.Tag == tag));
    }

    public static IEnumerable<TTagInstance> Tags<TTag, TTagInstance>(this IEnumerable<M3U8TagInstance> tags, TTag tag) where TTag : M3U8Tag where TTagInstance : M3U8TagInstance
    {
      return Enumerable.Where<TTagInstance>(Enumerable.OfType<TTagInstance>((IEnumerable) tags), (Func<TTagInstance, bool>) (t => t.Tag == (M3U8Tag) tag));
    }
  }
}
