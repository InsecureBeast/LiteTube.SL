using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Segments;

namespace LiteTube.StreamVideo.Playlists
{
  public class SubStreamSegment : ISegment
  {
    private readonly Uri _parentUrl;
    private readonly Uri _url;

    public Func<Stream, CancellationToken, Task<Stream>> AsyncStreamFilter { get; set; }

    public TimeSpan? Duration { get; set; }

    public long? MediaSequence { get; set; }

    public long Offset { get; set; }

    public long Length { get; set; }

    public Uri Url
    {
      get
      {
        return this._url;
      }
    }

    public Uri ParentUrl
    {
      get
      {
        return this._parentUrl;
      }
    }

    public SubStreamSegment(Uri url, Uri parentUrl)
    {
      if ((Uri) null == url)
        throw new ArgumentNullException("url");
      if ((Uri) null == parentUrl)
        throw new ArgumentNullException("parentUrl");
      this._url = url;
      this._parentUrl = parentUrl;
    }

    public Task<Stream> CreateFilterAsync(Stream stream, CancellationToken cancellationToken)
    {
      if (null == this.AsyncStreamFilter)
        return (Task<Stream>) null;
      return this.AsyncStreamFilter(stream, cancellationToken);
    }

    public override string ToString()
    {
      if (this.Length > 0L)
        return string.Format("{0} {1} {2} [offset {3} length {4}]", (object) this.MediaSequence, (object) this.Duration, (object) this.Url, (object) this.Offset, (object) (this.Offset + this.Length));
      return string.Format("{0} {1} {2}", (object) this.MediaSequence, (object) this.Duration, (object) this.Url);
    }
  }
}
