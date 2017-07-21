// Decompiled with JetBrains decompiler
// Type: SM.Media.Content.ContentServiceFactory`2
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Content
{
  public abstract class ContentServiceFactory<TService, TParameter> : IContentServiceFactory<TService, TParameter>
  {
    private static readonly Task<TService> NoHandler = TaskEx.FromResult<TService>(default (TService));
    private readonly IContentServiceFactoryFinder<TService, TParameter> _factoryFinder;

    protected ContentServiceFactory(IContentServiceFactoryFinder<TService, TParameter> factoryFinder)
    {
      if (null == factoryFinder)
        throw new ArgumentNullException("factoryFinder");
      this._factoryFinder = factoryFinder;
    }

    public virtual Task<TService> CreateAsync(TParameter parameter, ContentType contentType, CancellationToken cancellationToken)
    {
      if ((ContentType) null == contentType)
        throw new ArgumentNullException("contentType");
      IContentServiceFactoryInstance<TService, TParameter> factory = this._factoryFinder.GetFactory(contentType);
      if (null != factory)
        return factory.CreateAsync(parameter, contentType, cancellationToken);
      return ContentServiceFactory<TService, TParameter>.NoHandler;
    }
  }
}
