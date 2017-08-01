using System;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Web
{
  public interface IWebCacheManager : IDisposable
  {
    Task FlushAsync();

    Task<TCached> ReadAsync<TCached>(Uri uri, Func<Uri, byte[], TCached> factory, ContentKind contentKind, ContentType contentType, CancellationToken cancellationToken) where TCached : class;
  }
}
