// Decompiled with JetBrains decompiler
// Type: SM.Media.Content.ContentServiceFactoryFinder`2
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SM.Media.Content
{
  public class ContentServiceFactoryFinder<TService, TParameter> : IContentServiceFactoryFinder<TService, TParameter>
  {
    private volatile Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>> _factories;

    public ContentServiceFactoryFinder(IEnumerable<IContentServiceFactoryInstance<TService, TParameter>> factoryInstances)
    {
      this._factories = Enumerable.ToDictionary(Enumerable.SelectMany(factoryInstances, (Func<IContentServiceFactoryInstance<TService, TParameter>, IEnumerable<ContentType>>) (fi => (IEnumerable<ContentType>) fi.KnownContentTypes), (fi, contentType) =>
      {
        var fAnonymousType0 = new
        {
          ContentType = contentType,
          Instance = fi
        };
        return fAnonymousType0;
      }), v => v.ContentType, v => v.Instance);
    }

    public IContentServiceFactoryInstance<TService, TParameter> GetFactory(ContentType contentType)
    {
      IContentServiceFactoryInstance<TService, TParameter> serviceFactoryInstance;
      if (this._factories.TryGetValue(contentType, out serviceFactoryInstance))
        return serviceFactoryInstance;
      return (IContentServiceFactoryInstance<TService, TParameter>) null;
    }

    public void Register(ContentType contentType, IContentServiceFactoryInstance<TService, TParameter> factory)
    {
      this.SafeChangeFactories((Action<Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>>) (factories => factories[contentType] = factory));
    }

    public void Deregister(ContentType contentType)
    {
      this.SafeChangeFactories((Action<Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>>) (factories => factories.Remove(contentType)));
    }

    private void SafeChangeFactories(Action<Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>> changeAction)
    {
      Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>> comparand = this._factories;
      while (true)
      {
        Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>> dictionary1 = new Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>((IDictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>) comparand);
        changeAction(dictionary1);
        Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>> dictionary2 = Interlocked.CompareExchange<Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>>(ref this._factories, dictionary1, comparand);
        if (comparand != dictionary2)
          comparand = dictionary2;
        else
          break;
      }
    }
  }
}
