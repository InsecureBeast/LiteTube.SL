using System;
using System.Collections.Generic;
using System.Threading;

namespace LiteTube.StreamVideo.M3U8
{
  public class M3U8AttributeTag : M3U8Tag
  {
    private volatile IDictionary<string, M3U8Attribute> _attributes;

    public virtual IDictionary<string, M3U8Attribute> Attributes
    {
      get
      {
        return this._attributes;
      }
    }

    public M3U8AttributeTag(string name, M3U8TagScope scope, IDictionary<string, M3U8Attribute> attributes, Func<M3U8Tag, string, M3U8TagInstance> createInstance)
      : base(name, scope, createInstance)
    {
      this._attributes = attributes;
    }

    public void Register(M3U8Attribute attribute)
    {
      IDictionary<string, M3U8Attribute> dictionary1 = this._attributes;
      while (true)
      {
        Dictionary<string, M3U8Attribute> dictionary2 = new Dictionary<string, M3U8Attribute>(dictionary1);
        dictionary2[attribute.Name] = attribute;
        IDictionary<string, M3U8Attribute> dictionary3 = Interlocked.CompareExchange<IDictionary<string, M3U8Attribute>>(ref this._attributes, (IDictionary<string, M3U8Attribute>) dictionary2, dictionary1);
        if (dictionary3 != dictionary1)
          dictionary1 = dictionary3;
        else
          break;
      }
    }
  }
}
