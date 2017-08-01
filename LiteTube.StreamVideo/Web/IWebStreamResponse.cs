using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Web
{
  public interface IWebStreamResponse : IDisposable
  {
    bool IsSuccessStatusCode { get; }

    Uri ActualUrl { get; }

    int HttpStatusCode { get; }

    long? ContentLength { get; }

    void EnsureSuccessStatusCode();

    Task<Stream> GetStreamAsync(CancellationToken cancellationToken);
  }
}
