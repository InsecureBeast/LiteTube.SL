using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Content
{
    public interface IContentTypeDetector
    {
        ICollection<ContentType> GetContentType(Uri url, string mimeType = null, string fileName = null);
    }
}
