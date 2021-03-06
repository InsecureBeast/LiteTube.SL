﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public interface IHttpReader : IDisposable
  {
    bool HasData { get; }

    void Clear();

    Task<string> ReadLineAsync(CancellationToken cancellationToken);

    Task<int> ReadAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken);
  }
}
