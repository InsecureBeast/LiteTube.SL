using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LiteTube.StreamVideo.Content
{
    public class ContentServiceFactoryFinder<TService, TParameter> : IContentServiceFactoryFinder<TService, TParameter>
    {
        private volatile Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>> _factories;

        public ContentServiceFactoryFinder(IEnumerable<IContentServiceFactoryInstance<TService, TParameter>> factoryInstances)
        {
            this._factories = Enumerable.ToDictionary(Enumerable.SelectMany(factoryInstances, (Func<IContentServiceFactoryInstance<TService, TParameter>, IEnumerable<ContentType>>)(fi => (IEnumerable<ContentType>)fi.KnownContentTypes), (fi, contentType) =>
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
            return (IContentServiceFactoryInstance<TService, TParameter>)null;
        }

        public void Register(ContentType contentType, IContentServiceFactoryInstance<TService, TParameter> factory)
        {
            this.SafeChangeFactories((Action<Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>>)(factories => factories[contentType] = factory));
        }

        public void Deregister(ContentType contentType)
        {
            this.SafeChangeFactories((Action<Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>>)(factories => factories.Remove(contentType)));
        }

        private void SafeChangeFactories(Action<Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>> changeAction)
        {
            Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>> comparand = this._factories;
            while (true)
            {
                Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>> dictionary1 = new Dictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>((IDictionary<ContentType, IContentServiceFactoryInstance<TService, TParameter>>)comparand);
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
