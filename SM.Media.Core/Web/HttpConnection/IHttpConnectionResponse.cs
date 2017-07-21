using System;
using System.IO;
using System.Linq;

namespace SM.Media.Core.Web.HttpConnection
{
  public interface IHttpConnectionResponse : IDisposable
  {
    ILookup<string, string> Headers { get; }

    Stream ContentReadStream { get; }

    IHttpStatus Status { get; }

    Uri ResponseUri { get; }

    bool IsSuccessStatusCode { get; }

    void EnsureSuccessStatusCode();
  }
}
