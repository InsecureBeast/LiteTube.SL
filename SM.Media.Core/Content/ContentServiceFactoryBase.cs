using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Content
{
  public abstract class ContentServiceFactoryBase<TServiceImplementation, TService, TParameter> : IContentServiceFactoryInstance<TService, TParameter> where TServiceImplementation : TService
  {
    public abstract ICollection<ContentType> KnownContentTypes { get; }

    public virtual Task<TService> CreateAsync(TParameter parameter, ContentType contentType, CancellationToken cancellationToken)
    {
      return TaskEx.FromResult<TService>((TService) this.Create(parameter, contentType));
    }

    protected abstract TServiceImplementation Create(TParameter parameter, ContentType contentType);
  }
}
