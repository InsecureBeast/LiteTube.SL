﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.SimpleSegmentManagerBase
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.Content;
using SM.Media.Metadata;
using SM.Media.Utility;
using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Segments
{
  public class SimpleSegmentManagerBase : ISegmentManager, IDisposable, IAsyncEnumerable<ISegment>
  {
    private static readonly Task<TimeSpan> TimeSpanZeroTask = TaskEx.FromResult<TimeSpan>(TimeSpan.Zero);
    private readonly ContentType _contentType;
    private readonly ICollection<ISegment> _segments;
    private readonly IWebReader _webReader;
    private int _isDisposed;

    public IStreamMetadata StreamMetadata
    {
      get
      {
        Uri uri = this._webReader.BaseAddress;
        if ((Uri) null == uri)
        {
          ISegment segment = Enumerable.FirstOrDefault<ISegment>((IEnumerable<ISegment>) this._segments);
          if (null != segment)
            uri = segment.Url;
        }
        return (IStreamMetadata) new SM.Media.Metadata.StreamMetadata()
        {
          Url = uri,
          ContentType = this._contentType,
          Duration = this.Duration
        };
      }
    }

    public IWebReader WebReader
    {
      get
      {
        return this._webReader;
      }
    }

    public TimeSpan StartPosition
    {
      get
      {
        return TimeSpan.Zero;
      }
    }

    public TimeSpan? Duration
    {
      get
      {
        return new TimeSpan?();
      }
    }

    public ContentType ContentType
    {
      get
      {
        return this._contentType;
      }
    }

    public IAsyncEnumerable<ISegment> Playlist
    {
      get
      {
        return (IAsyncEnumerable<ISegment>) this;
      }
    }

    protected SimpleSegmentManagerBase(IWebReader webReader, ICollection<ISegment> segments, ContentType contentType)
    {
      if (null == webReader)
        throw new ArgumentNullException("webReader");
      if (null == segments)
        throw new ArgumentNullException("segments");
      this._webReader = webReader;
      this._contentType = contentType;
      this._segments = segments;
    }

    public IAsyncEnumerator<ISegment> GetEnumerator()
    {
      return (IAsyncEnumerator<ISegment>) new SimpleSegmentManagerBase.SimpleEnumerator((IEnumerable<ISegment>) this._segments);
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public Task<TimeSpan> SeekAsync(TimeSpan timestamp)
    {
      return SimpleSegmentManagerBase.TimeSpanZeroTask;
    }

    public Task StartAsync()
    {
      return TplTaskExtensions.CompletedTask;
    }

    public Task StopAsync()
    {
      return TplTaskExtensions.CompletedTask;
    }

    public Task CloseAsync()
    {
      return TplTaskExtensions.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this._webReader.Dispose();
    }

    private class SimpleEnumerator : IAsyncEnumerator<ISegment>, IDisposable
    {
      private readonly IEnumerator<ISegment> _enumerator;

      public ISegment Current { get; private set; }

      public SimpleEnumerator(IEnumerable<ISegment> segments)
      {
        this._enumerator = segments.GetEnumerator();
      }

      public void Dispose()
      {
        using (this._enumerator)
          ;
      }

      public Task<bool> MoveNextAsync()
      {
        if (!this._enumerator.MoveNext())
          return TplTaskExtensions.FalseTask;
        this.Current = this._enumerator.Current;
        return TplTaskExtensions.TrueTask;
      }
    }
  }
}
