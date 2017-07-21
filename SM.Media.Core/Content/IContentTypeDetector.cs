using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Content
{
    public interface IContentTypeDetector
    {
        ICollection<ContentType> GetContentType(Uri url, string mimeType = null, string fileName = null);
    }
}
