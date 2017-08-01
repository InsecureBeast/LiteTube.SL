using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Segments
{
  public sealed class SegmentReaderManager : ISegmentReaderManager, IDisposable
  {
    private readonly ISegmentManager[] _segmentManagers;
    private readonly SegmentReaderManager.ManagerReaders[] _segmentReaders;

    public ICollection<ISegmentManagerReaders> SegmentManagerReaders
    {
      get
      {
        return (ICollection<ISegmentManagerReaders>) this._segmentReaders;
      }
    }

    public TimeSpan? Duration
    {
      get
      {
        return Enumerable.Max<ISegmentManager, TimeSpan?>((IEnumerable<ISegmentManager>) this._segmentManagers, (Func<ISegmentManager, TimeSpan?>) (sm => sm.Duration));
      }
    }

    public SegmentReaderManager(IEnumerable<ISegmentManager> segmentManagers, IWebMetadataFactory webMetadataFactory, IRetryManager retryManager, IPlatformServices platformServices)
    {
      if (null == segmentManagers)
        throw new ArgumentNullException("segmentManagers");
      if (null == webMetadataFactory)
        throw new ArgumentNullException("webMetadataFactory");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._segmentManagers = Enumerable.ToArray<ISegmentManager>(segmentManagers);
      if (this._segmentManagers.Length < 1)
        throw new ArgumentException("No segment managers provided");
      this._segmentReaders = Enumerable.ToArray<SegmentReaderManager.ManagerReaders>(Enumerable.Select<ISegmentManager, SegmentReaderManager.ManagerReaders>((IEnumerable<ISegmentManager>) this._segmentManagers, (Func<ISegmentManager, SegmentReaderManager.ManagerReaders>) (sm => new SegmentReaderManager.ManagerReaders()
      {
        Manager = sm,
        Readers = (IAsyncEnumerable<ISegmentReader>) new SegmentReaderManager.SegmentReaderEnumerable(sm, webMetadataFactory, retryManager, platformServices)
      })));
    }

    public void Dispose()
    {
      if (null == this._segmentManagers)
        return;
      foreach (ISegmentManager segmentManager in this._segmentManagers)
      {
        try
        {
        }
        finally
        {
          if (segmentManager != null)
            segmentManager.Dispose();
        }
      }
    }

    public Task StartAsync()
    {
      return TaskEx.WhenAll(Enumerable.Select<ISegmentManager, Task>((IEnumerable<ISegmentManager>) this._segmentManagers, (Func<ISegmentManager, Task>) (sm => sm.StartAsync())));
    }

    public Task StopAsync()
    {
      return TaskEx.WhenAll(Enumerable.Select<ISegmentManager, Task>((IEnumerable<ISegmentManager>) this._segmentManagers, (Func<ISegmentManager, Task>) (sm => sm.StopAsync())));
    }

    public async Task<TimeSpan> SeekAsync(TimeSpan timestamp, CancellationToken cancellationToken)
    {
      IEnumerable<Task<TimeSpan>> tasks = Enumerable.Select<ISegmentManager, Task<TimeSpan>>((IEnumerable<ISegmentManager>) this._segmentManagers, (Func<ISegmentManager, Task<TimeSpan>>) (sm => sm.SeekAsync(timestamp)));
      TimeSpan[] results = await TaskEx.WhenAll<TimeSpan>(tasks).ConfigureAwait(false);
      return Enumerable.Min<TimeSpan>((IEnumerable<TimeSpan>) results);
    }

    private class ManagerReaders : ISegmentManagerReaders
    {
      public ISegmentManager Manager { get; set; }

      public IAsyncEnumerable<ISegmentReader> Readers { get; set; }
    }

    private class SegmentReaderEnumerable : IAsyncEnumerable<ISegmentReader>
    {
      private readonly IPlatformServices _platformServices;
      private readonly IRetryManager _retryManager;
      private readonly ISegmentManager _segmentManager;
      private readonly IWebMetadataFactory _webMetadataFactory;

      public SegmentReaderEnumerable(ISegmentManager segmentManager, IWebMetadataFactory webMetadataFactory, IRetryManager retryManager, IPlatformServices platformServices)
      {
        if (null == segmentManager)
          throw new ArgumentNullException("segmentManager");
        if (null == webMetadataFactory)
          throw new ArgumentNullException("webMetadataFactory");
        if (null == platformServices)
          throw new ArgumentNullException("platformServices");
        if (null == retryManager)
          throw new ArgumentNullException("retryManager");
        this._segmentManager = segmentManager;
        this._webMetadataFactory = webMetadataFactory;
        this._platformServices = platformServices;
        this._retryManager = retryManager;
      }

      public IAsyncEnumerator<ISegmentReader> GetEnumerator()
      {
        return (IAsyncEnumerator<ISegmentReader>) new SegmentReaderManager.SegmentReaderEnumerator(this._segmentManager, this._webMetadataFactory, this._retryManager, this._platformServices);
      }
    }

    private class SegmentReaderEnumerator : IAsyncEnumerator<ISegmentReader>, IDisposable
    {
      private readonly IPlatformServices _platformServices;
      private readonly IRetryManager _retryManager;
      private readonly IAsyncEnumerator<ISegment> _segmentEnumerator;
      private readonly IWebMetadataFactory _webMetadataFactory;
      private readonly IWebReader _webReader;
      private ISegmentReader _segmentReader;

      public ISegmentReader Current
      {
        get
        {
          return this._segmentReader;
        }
      }

      public SegmentReaderEnumerator(ISegmentManager segmentManager, IWebMetadataFactory webMetadataFactory, IRetryManager retryManager, IPlatformServices platformServices)
      {
        if (null == segmentManager)
          throw new ArgumentNullException("segmentManager");
        if (null == webMetadataFactory)
          throw new ArgumentNullException("webMetadataFactory");
        if (null == retryManager)
          throw new ArgumentNullException("retryManager");
        if (null == platformServices)
          throw new ArgumentNullException("platformServices");
        this._segmentEnumerator = segmentManager.Playlist.GetEnumerator();
        this._webReader = segmentManager.WebReader;
        this._webMetadataFactory = webMetadataFactory;
        this._retryManager = retryManager;
        this._platformServices = platformServices;
      }

      public void Dispose()
      {
        this.CloseReader();
        using (this._segmentEnumerator)
          ;
      }

      public async Task<bool> MoveNextAsync()
      {
        this.CloseReader();
        bool flag;
        if (!await this._segmentEnumerator.MoveNextAsync().ConfigureAwait(false))
        {
          flag = false;
        }
        else
        {
          ISegment segment = this._segmentEnumerator.Current;
          this._segmentReader = (ISegmentReader) new SegmentReader(segment, this._webReader, this._webMetadataFactory, this._retryManager, this._platformServices);
          flag = true;
        }
        return flag;
      }

      private void CloseReader()
      {
        ISegmentReader segmentReader = this._segmentReader;
        this._segmentReader = (ISegmentReader) null;
        using (segmentReader)
          ;
      }
    }
  }
}
