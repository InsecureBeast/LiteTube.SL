using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Web
{
    public static class HttpClientContentTypeDetectorExtensions
    {
        public static ICollection<ContentType> GetContentType(this IContentTypeDetector contentTypeDetector, Uri url, HttpContentHeaders headers, string fileName)
        {
            string mimeType = (string)null;
            MediaTypeHeaderValue contentType = headers.ContentType;
            if (null != contentType)
                mimeType = contentType.MediaType;
            return contentTypeDetector.GetContentType(url, mimeType, fileName);
        }
    }
}
