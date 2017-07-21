using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Content
{
    public abstract class ContentServiceFactory<TService, TParameter> : IContentServiceFactory<TService, TParameter>
    {
        private static readonly Task<TService> NoHandler = TaskEx.FromResult<TService>(default(TService));
        private readonly IContentServiceFactoryFinder<TService, TParameter> _factoryFinder;

        protected ContentServiceFactory(IContentServiceFactoryFinder<TService, TParameter> factoryFinder)
        {
            if (null == factoryFinder)
                throw new ArgumentNullException("factoryFinder");
            this._factoryFinder = factoryFinder;
        }

        public virtual Task<TService> CreateAsync(TParameter parameter, ContentType contentType, CancellationToken cancellationToken)
        {
            if ((ContentType)null == contentType)
                throw new ArgumentNullException("contentType");
            IContentServiceFactoryInstance<TService, TParameter> factory = this._factoryFinder.GetFactory(contentType);
            if (null != factory)
                return factory.CreateAsync(parameter, contentType, cancellationToken);
            return ContentServiceFactory<TService, TParameter>.NoHandler;
        }
    }
}
