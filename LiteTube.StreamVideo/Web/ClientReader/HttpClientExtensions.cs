﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Web.ClientReader
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> SendAsync(this HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken, Uri referrer, long? fromBytes, long? toBytes)
        {
            if ((Uri)null != referrer)
                request.Headers.Referrer = referrer;
            if (fromBytes.HasValue || toBytes.HasValue)
                request.Headers.Range = new RangeHeaderValue(fromBytes, toBytes);
            return await httpClient.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
        }
    }
}
