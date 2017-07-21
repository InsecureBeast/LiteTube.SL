﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.SimpleSegment
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Segments
{
  public class SimpleSegment : ISegment
  {
    private readonly Uri _parentUrl;
    private readonly Uri _url;

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

    public long Offset { get; set; }

    public long Length { get; set; }

    public TimeSpan? Duration { get; set; }

    public long? MediaSequence { get; set; }

    public SimpleSegment(Uri url, Uri parentUrl)
    {
      if ((Uri) null == url)
        throw new ArgumentNullException("url");
      this._url = url;
      this._parentUrl = parentUrl;
    }

    public Task<Stream> CreateFilterAsync(Stream stream, CancellationToken cancellationToken)
    {
      return (Task<Stream>) null;
    }
  }
}
