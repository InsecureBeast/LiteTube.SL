using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public interface ISocket : IDisposable
  {
    Task ConnectAsync(Uri url, CancellationToken cancellationToken);

    Task<int> WriteAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken);

    Task<int> ReadAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken);

    void Close();
  }
}
