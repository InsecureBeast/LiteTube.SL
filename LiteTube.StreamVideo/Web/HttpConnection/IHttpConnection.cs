using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public interface IHttpConnection : IDisposable
  {
    Task ConnectAsync(Uri url, CancellationToken cancellationToken);

    Task<IHttpConnectionResponse> GetAsync(HttpConnectionRequest request, bool closeConnection, CancellationToken cancellationToken);

    void Close();
  }
}
