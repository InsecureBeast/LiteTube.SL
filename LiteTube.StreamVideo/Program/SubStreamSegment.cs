using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Segments;

namespace LiteTube.StreamVideo.Program
{
    public class SubStreamSegment : ISegment
    {
        public SubStreamSegment(Uri url, Uri parentUrl)
        {
            if (null == url)
                throw new ArgumentNullException(nameof(url));
            if (null == parentUrl)
                throw new ArgumentNullException(nameof(parentUrl));
            Url = url;
            ParentUrl = parentUrl;
        }

        public Func<Stream, CancellationToken, Task<Stream>> AsyncStreamFilter { get; set; }
        public TimeSpan? Duration { get; set; }
        public long? MediaSequence { get; set; }
        public long Offset { get; set; }
        public long Length { get; set; }
        public Uri Url { get; }
        public Uri ParentUrl { get; }

        public Task<Stream> CreateFilterAsync(Stream stream, CancellationToken cancellationToken)
        {
            return AsyncStreamFilter?.Invoke(stream, cancellationToken);
        }

        public override string ToString()
        {
            return Length > 0 
                ? $"{ MediaSequence} { Duration} { Url} [offset { Offset} length { Offset + Length}]" 
                : $"{ MediaSequence} { Duration} { Url}";
        }
    }
}
