﻿using System;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;

namespace SM.Media.Core.Web
{
  public interface IWebReaderManager
  {
    IWebReader CreateReader(Uri url, ContentKind contentKind, IWebReader parent = null, ContentType contentType = null);

    IWebCache CreateWebCache(Uri url, ContentKind contentKind, IWebReader parent = null, ContentType contentType = null);

    Task<ContentType> DetectContentTypeAsync(Uri url, ContentKind contentKind, CancellationToken cancellationToken, IWebReader parent = null);
  }
}
