﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.WebReaderExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.Utility;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web
{
  public static class WebReaderExtensions
  {
    public static IWebReader CreateChild(this IWebReader webReader, Uri url, ContentKind contentKind, ContentType contentType = null)
    {
      return webReader.Manager.CreateReader(url, contentKind, webReader, contentType);
    }

    public static IWebCache CreateWebCache(this IWebReader webReader, Uri url, ContentKind contentKind, ContentType contentType = null)
    {
      return webReader.Manager.CreateWebCache(url, contentKind, webReader, contentType);
    }

    public static Task<ContentType> DetectContentTypeAsync(this IWebReader webReader, Uri url, ContentKind contentKind, CancellationToken cancellationToken)
    {
      return webReader.Manager.DetectContentTypeAsync(url, contentKind, cancellationToken, webReader);
    }

    public static async Task<TReturn> ReadStreamAsync<TReturn>(this IWebReader webReader, Uri url, IRetry retry, Func<Uri, Stream, TReturn> reader, CancellationToken cancellationToken)
    {
      TReturn @return;
      while (true)
      {
        using (IWebStreamResponse webStreamResponse = await webReader.GetWebStreamAsync(url, true, cancellationToken, (Uri) null, new long?(), new long?(), (WebResponse) null).ConfigureAwait(false))
        {
          if (webStreamResponse.IsSuccessStatusCode)
          {
            Uri actualUrl = webStreamResponse.ActualUrl;
            using (Stream stream = await webStreamResponse.GetStreamAsync(cancellationToken).ConfigureAwait(false))
            {
              using (MemoryStream memoryStream = new MemoryStream((int) (webStreamResponse.ContentLength ?? 4096L)))
              {
                await stream.CopyToAsync((Stream) memoryStream, 4096, cancellationToken).ConfigureAwait(false);
                memoryStream.Position = 0L;
                @return = reader(actualUrl, (Stream) memoryStream);
                break;
              }
            }
          }
          else
          {
            if (!RetryPolicy.IsRetryable((HttpStatusCode) webStreamResponse.HttpStatusCode))
              webStreamResponse.EnsureSuccessStatusCode();
            bool canRetry = await retry.CanRetryAfterDelayAsync(cancellationToken).ConfigureAwait(false);
            if (!canRetry)
              webStreamResponse.EnsureSuccessStatusCode();
          }
        }
      }
      return @return;
    }

    public static async Task<TReturn> ReadStreamAsync<TReturn>(this IWebReader webReader, Uri url, Retry retry, Func<Uri, Stream, CancellationToken, Task<TReturn>> reader, CancellationToken cancellationToken)
    {
      TReturn @return;
      while (true)
      {
        using (IWebStreamResponse webStreamAsync = await webReader.GetWebStreamAsync(url, false, cancellationToken, (Uri) null, new long?(), new long?(), (WebResponse) null))
        {
          if (webStreamAsync.IsSuccessStatusCode)
          {
            Uri actualUrl = webStreamAsync.ActualUrl;
            using (Stream stream = await webStreamAsync.GetStreamAsync(cancellationToken).ConfigureAwait(false))
            {
              @return = await reader(actualUrl, stream, cancellationToken).ConfigureAwait(false);
              break;
            }
          }
          else
          {
            if (!RetryPolicy.IsRetryable((HttpStatusCode) webStreamAsync.HttpStatusCode))
              webStreamAsync.EnsureSuccessStatusCode();
            bool canRetry = await retry.CanRetryAfterDelayAsync(cancellationToken).ConfigureAwait(false);
            if (!canRetry)
              webStreamAsync.EnsureSuccessStatusCode();
          }
        }
      }
      return @return;
    }
  }
}
