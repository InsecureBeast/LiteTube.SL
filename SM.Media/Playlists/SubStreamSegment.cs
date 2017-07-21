// Decompiled with JetBrains decompiler
// Type: SM.Media.Playlists.SubStreamSegment
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Segments;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Playlists
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
