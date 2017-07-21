using System;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;

namespace SM.Media.Core.Web
{
  public interface IWebCacheManager : IDisposable
  {
    Task FlushAsync();

    Task<TCached> ReadAsync<TCached>(Uri uri, Func<Uri, byte[], TCached> factory, ContentKind contentKind, ContentType contentType, CancellationToken cancellationToken) where TCached : class;
  }
}
