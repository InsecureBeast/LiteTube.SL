﻿using System;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;

namespace SM.Media.Core.Web
{
  public interface IWebReader : IDisposable
  {
    Uri BaseAddress { get; }

    Uri RequestUri { get; }

    ContentType ContentType { get; }

    IWebReaderManager Manager { get; }

    Task<IWebStreamResponse> GetWebStreamAsync(Uri url, bool waitForContent, CancellationToken cancellationToken, Uri referrer = null, long? from = null, long? to = null, WebResponse response = null);

    Task<byte[]> GetByteArrayAsync(Uri url, CancellationToken cancellationToken, WebResponse webResponse = null);
  }
}
