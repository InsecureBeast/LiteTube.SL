// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.IWebReader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web
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
