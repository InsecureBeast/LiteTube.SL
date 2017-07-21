using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Metadata;
using SM.Media.Core.Playlists;
using SM.Media.Core.Segments;
using SM.Media.Core.Utility;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
{
  public sealed class HlsPlaylistSegmentManager : ISegmentManager, IDisposable
  {
    private readonly List<ISegment[]> _dynamicPlaylists = new List<ISegment[]>();
    private readonly TaskTimer _refreshTimer = new TaskTimer();
    private readonly List<ISegment> _segmentList = new List<ISegment>();
    private readonly object _segmentLock = new object();
    private int _startSegmentIndex = -1;
    private readonly CancellationToken _cancellationToken;
    private readonly TimeSpan _excessiveDuration;
    private readonly TimeSpan _maximumReload;
    private readonly TimeSpan _minimumReload;
    private readonly TimeSpan _minimumRetry;
    private readonly IPlatformServices _platformServices;
    private readonly IProgramStream _programStream;
    private CancellationTokenSource _abortTokenSource;
    private int _dynamicStartIndex;
    private int _isDisposed;
    private bool _isDynamicPlaylist;
    private bool _isInitialized;
    private bool _isRunning;
    private int _readSubListFailureCount;
    private SignalTask _readTask;
    private ISegment[] _segments;
    private int _segmentsExpiration;

    private CancellationToken CancellationToken
    {
      get
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._segmentLock, ref lockTaken);
          return this._abortTokenSource == null ? CancellationToken.None : this._abortTokenSource.Token;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
    }

    public IWebReader WebReader { get; private set; }

    public TimeSpan StartPosition { get; private set; }

    public TimeSpan? Duration { get; private set; }

    public ContentType ContentType { get; private set; }

    public IAsyncEnumerable<ISegment> Playlist { get; private set; }

    public IStreamMetadata StreamMetadata
    {
      get
      {
        return this._programStream.StreamMetadata;
      }
    }

    public HlsPlaylistSegmentManager(IProgramStream programStream, IPlatformServices platformServices, CancellationToken cancellationToken)
    {
      if (null == programStream)
        throw new ArgumentNullException("programStream");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._programStream = programStream;
      this._platformServices = platformServices;
      this._cancellationToken = cancellationToken;
      HlsPlaylistParameters parameters = HlsPlaylistSettings.Parameters;
      this._minimumRetry = parameters.MinimumRetry;
      this._minimumReload = parameters.MinimumReload;
      this._maximumReload = parameters.MaximumReload;
      this._excessiveDuration = parameters.ExcessiveDuration;
      this._abortTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._cancellationToken);
      this._isDynamicPlaylist = true;
      this._readTask = new SignalTask(new Func<Task>(this.ReadSubList), this._abortTokenSource.Token);
      this._segmentsExpiration = Environment.TickCount;
      this.Playlist = (IAsyncEnumerable<ISegment>) new HlsPlaylistSegmentManager.PlaylistEnumerable(this);
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this._refreshTimer.Cancel();
      try
      {
        this.CleanupReader().Wait();
      }
      catch (Exception ex)
      {
        Debug.WriteLine("HlsPlaylistSegmentManager.Dispose() failed: " + ex.Message);
      }
      this._refreshTimer.Dispose();
    }

    public async Task StartAsync()
    {
      this.ThrowIfDisposed();
      this._cancellationToken.ThrowIfCancellationRequested();
      SignalTask oldReadTask = (SignalTask) null;
      CancellationTokenSource cancellationTokenSource = (CancellationTokenSource) null;
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._segmentLock, ref lockTaken);
        if (!this._isRunning)
        {
          if (this._abortTokenSource.IsCancellationRequested)
          {
            cancellationTokenSource = this._abortTokenSource;
            this._abortTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._cancellationToken);
            oldReadTask = this._readTask;
            this._readTask = new SignalTask(new Func<Task>(this.ReadSubList), this._abortTokenSource.Token);
          }
          this._isRunning = true;
        }
        else
          goto label_15;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      await HlsPlaylistSegmentManager.CleanupReader(oldReadTask, cancellationTokenSource).ConfigureAwait(false);
      if (this._programStream.Segments != null && this._programStream.Segments.Count > 0)
        this.UpdatePlaylist();
      ISegment segment = await AsyncEnumerableExtensions.FirstOrDefaultAsync<ISegment>(this.Playlist).ConfigureAwait(false);
      if (null == segment)
      {
        Debug.WriteLine("HlsPlaylistSegmentManager.StartAsync() no segments found");
        throw new FileNotFoundException("Unable to find the first segment");
      }
      this.ContentType = await this._programStream.GetContentTypeAsync(this._cancellationToken).ConfigureAwait(false);
      this.WebReader = WebReaderExtensions.CreateChild(this._programStream.WebReader, (Uri) null, ContentKind.AnyMedia, this.ContentType);
label_15:;
    }

    public Task StopAsync()
    {
      if (0 != this._isDisposed)
        return TplTaskExtensions.CompletedTask;
      this._refreshTimer.Cancel();
      bool lockTaken = false;
      object obj = null;
      CancellationTokenSource cancellationTokenSource;
      SignalTask signalTask;
      try
      {
        Monitor.Enter(obj = this._segmentLock, ref lockTaken);
        this._isRunning = false;
        cancellationTokenSource = this._abortTokenSource;
        signalTask = this._readTask;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      CancellationTokenExtensions.CancelSafe(cancellationTokenSource);
      return signalTask.WaitAsync();
    }

    public async Task<TimeSpan> SeekAsync(TimeSpan timestamp)
    {
      this.ThrowIfDisposed();
      this.CancellationToken.ThrowIfCancellationRequested();
      if (this._isDynamicPlaylist)
        await this.CheckReload(-1).ConfigureAwait(false);
      bool lockTaken = false;
      TimeSpan actualPosition;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._segmentLock, ref lockTaken);
        this.Seek(timestamp);
        actualPosition = this.StartPosition;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      return actualPosition;
    }

    private async Task CleanupReader()
    {
      bool lockTaken1 = false;
      SignalTask readTask;
      CancellationTokenSource cancellationTokenSource;
      object obj1 = null;
      try
      {
        Monitor.Enter(obj1 = this._segmentLock, ref lockTaken1);
        readTask = this._readTask;
        cancellationTokenSource = this._abortTokenSource;
        this._abortTokenSource = (CancellationTokenSource) null;
      }
      finally
      {
        if (lockTaken1)
          Monitor.Exit(obj1);
      }
      await HlsPlaylistSegmentManager.CleanupReader(readTask, cancellationTokenSource).ConfigureAwait(false);
      bool lockTaken2 = false;
      object obj2 = null;
      try
      {
        Monitor.Enter(obj2 = this._segmentLock, ref lockTaken2);
        this._readTask = (SignalTask) null;
      }
      finally
      {
        if (lockTaken2)
          Monitor.Exit(obj2);
      }
    }

    private static async Task CleanupReader(SignalTask readTask, CancellationTokenSource cancellationTokenSource)
    {
      using (cancellationTokenSource)
      {
        using (readTask)
        {
          if (null != cancellationTokenSource)
          {
            try
            {
              if (!cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
              Debug.WriteLine("HlsPlaylistSegmentManager.CleanupReader() cancel failed: " + ex.Message);
            }
          }
          if (null != readTask)
            await readTask.WaitAsync().ConfigureAwait(false);
        }
      }
    }

    private void ThrowIfDisposed()
    {
      if (0 != this._isDisposed)
        throw new ObjectDisposedException(this.GetType().FullName);
    }

    private void Seek(TimeSpan timestamp)
    {
      this.StartPosition = TimeSpan.Zero;
      this._startSegmentIndex = this._isDynamicPlaylist ? this._dynamicStartIndex : -1;
      if (this._segments == null || this._segments.Length < 1 || timestamp <= TimeSpan.Zero)
        return;
      TimeSpan timeSpan = TimeSpan.Zero;
      for (int index = 0; index < this._segments.Length; ++index)
      {
        TimeSpan? saneDuration = this.GetSaneDuration(this._segments[index].Duration);
        if (saneDuration.HasValue)
        {
          if (timeSpan + saneDuration.Value > timestamp)
          {
            this.StartPosition = timeSpan;
            this._startSegmentIndex = index - 1;
            return;
          }
          timeSpan += saneDuration.Value;
        }
        else
          break;
      }
      this.StartPosition = timeSpan;
      this._startSegmentIndex = this._segments.Length;
    }

    private Task CheckReload(int index)
    {
      this.CancellationToken.ThrowIfCancellationRequested();
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._segmentLock, ref lockTaken);
        if (this._isInitialized && (!this._isDynamicPlaylist || !this._isRunning || this._segmentsExpiration - Environment.TickCount > 0))
          return TplTaskExtensions.CompletedTask;
        this._readTask.Fire();
        if (this._segments == null || index + 1 >= this._segments.Length)
          return this._readTask.WaitAsync();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      return TplTaskExtensions.CompletedTask;
    }

    private async Task ReadSubList()
    {
      Debug.WriteLine("HlsPlaylistSegmentManager.ReadSubList({0})", (object) DateTimeOffset.Now);
      try
      {
        this._refreshTimer.Cancel();
        DateTime start = DateTime.UtcNow;
        await this._programStream.RefreshPlaylistAsync(this._cancellationToken).ConfigureAwait(false);
        TimeSpan fetchElapsed = DateTime.UtcNow - start;
        Debug.WriteLine("HlsPlaylistSegmentManager.ReadSubList() refreshed playlist in " + (object) fetchElapsed);
        if (this.UpdatePlaylist())
        {
          this._readSubListFailureCount = 0;
          if (this._isDynamicPlaylist)
          {
            TimeSpan expiration = TimeSpan.FromMilliseconds((double) (this._segmentsExpiration - Environment.TickCount));
            if (expiration < this._minimumRetry)
            {
              Debug.WriteLine("Expiration too short: " + (object) expiration);
              expiration = this._minimumRetry;
            }
            this.CancellationToken.ThrowIfCancellationRequested();
            this._refreshTimer.SetTimer((Action) (() => this._readTask.Fire()), expiration);
            goto label_16;
          }
          else
            goto label_16;
        }
      }
      catch (OperationCanceledException ex)
      {
        goto label_16;
      }
      catch (Exception ex)
      {
        Debug.WriteLine("HlsPlaylistSegmentManager.ReadSubList() failed: " + ex.Message);
      }
      if (++this._readSubListFailureCount > 3)
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._segmentLock, ref lockTaken);
          this._segments = (ISegment[]) null;
          this._isDynamicPlaylist = false;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
      else
      {
        double delay = 1.0 + (double) (1 << 2 * this._readSubListFailureCount);
        delay += delay / 2.0 * (this._platformServices.GetRandomNumber() - 0.5);
        TimeSpan timeSpan = TimeSpan.FromSeconds(delay);
        Debug.WriteLine("HlsPlaylistSegmentManager.ReadSubList(): retrying update in " + (object) timeSpan);
        this.CancellationToken.ThrowIfCancellationRequested();
        this._refreshTimer.SetTimer((Action) (() => this._readTask.Fire()), timeSpan);
      }
label_16:;
    }

    private bool UpdatePlaylist()
    {
      ISegment[] segments = Enumerable.ToArray<ISegment>((IEnumerable<ISegment>) this._programStream.Segments);
      bool isDynamicPlaylist = this._programStream.IsDynamicPlaylist;
      this.Duration = isDynamicPlaylist ? new TimeSpan?() : this.GetDuration((IEnumerable<ISegment>) segments);
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._segmentLock, ref lockTaken);
        if (!this._isRunning)
          return true;
        this.UnlockedUpdatePlaylist(isDynamicPlaylist, segments);
        this._isDynamicPlaylist = isDynamicPlaylist;
        this._isInitialized = true;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      Debug.WriteLine("HlsPlaylistSegmentManager.UpdatePlaylist: playlist {0} loaded with {1} entries. index: {2} dynamic: {3} expires: {4} ({5})", (object) this._programStream, (object) this._segments.Length, (object) this._startSegmentIndex, (object) (bool) (isDynamicPlaylist), (object) (isDynamicPlaylist ? TimeSpan.FromMilliseconds((double) (this._segmentsExpiration - Environment.TickCount)) : TimeSpan.Zero), (object) DateTimeOffset.Now);
      return true;
    }

    private void UnlockedUpdatePlaylist(bool isDynamicPlaylist, ISegment[] segments)
    {
      bool flag = false;
      if (isDynamicPlaylist || this._dynamicPlaylists.Count > 0)
      {
        ISegment[] segmentArray = Enumerable.LastOrDefault<ISegment[]>((IEnumerable<ISegment[]>) this._dynamicPlaylists);
        if (segments.Length > 0)
        {
          if (segmentArray != null && HlsPlaylistSegmentManager.SegmentsMatch(segmentArray[0], segments[0]) && segmentArray.Length == segments.Length)
          {
            Debug.WriteLine("HlsPlaylistSegmentManager.UpdatePlaylist(): need reload ({0})", (object) DateTimeOffset.Now);
            int num = Environment.TickCount + (int) Math.Round(2.0 * this._minimumRetry.TotalMilliseconds);
            if (this._segmentsExpiration < num)
              this._segmentsExpiration = num;
            flag = true;
          }
          else
          {
            this._dynamicPlaylists.Add(segments);
            if (this._dynamicPlaylists.Count > 4)
              this._dynamicPlaylists.RemoveAt(0);
          }
        }
        if (!flag)
        {
          segments = this.ResyncSegments();
          if (isDynamicPlaylist)
            this.UpdateDynamicPlaylistExpiration((IList<ISegment>) segments);
        }
      }
      if (flag)
        return;
      this._segments = segments;
    }

    private ISegment[] ResyncSegments()
    {
      ISegment[] segmentArray1 = (ISegment[]) null;
      int num = -1;
      foreach (ISegment[] segmentArray2 in this._dynamicPlaylists)
      {
        num = segmentArray2.Length;
        if (null == segmentArray1)
        {
          this._segmentList.AddRange((IEnumerable<ISegment>) segmentArray2);
          segmentArray1 = segmentArray2;
        }
        else
        {
          bool flag = false;
          for (int index1 = 0; index1 < segmentArray1.Length; ++index1)
          {
            if (HlsPlaylistSegmentManager.SegmentsMatch(segmentArray1[index1], segmentArray2[0]))
            {
              for (int index2 = 1; index2 < segmentArray2.Length; ++index2)
              {
                if (index1 + index2 >= segmentArray1.Length || !HlsPlaylistSegmentManager.SegmentsMatch(segmentArray1[index1 + index2], segmentArray2[index2]))
                  this._segmentList.Add(segmentArray2[index2]);
              }
              flag = true;
              break;
            }
          }
          if (!flag)
            this._segmentList.AddRange((IEnumerable<ISegment>) segmentArray2);
          segmentArray1 = segmentArray2;
        }
      }
      ISegment[] segmentArray3 = this._segmentList.ToArray();
      this._segmentList.Clear();
      this.SetDynamicStartIndex((IList<ISegment>) segmentArray3, segmentArray3.Length - num - 1);
      return segmentArray3;
    }

    private static bool SegmentsMatch(ISegment a, ISegment b)
    {
      long? mediaSequence;
      int num1;
      if (a.MediaSequence.HasValue)
      {
        mediaSequence = b.MediaSequence;
        num1 = !mediaSequence.HasValue ? 1 : 0;
      }
      else
        num1 = 1;
      if (num1 != 0)
        return a.Url == b.Url;
      mediaSequence = a.MediaSequence;
      long num2 = mediaSequence.Value;
      mediaSequence = b.MediaSequence;
      long num3 = mediaSequence.Value;
      return num2 == num3;
    }

    private void SetDynamicStartIndex(IList<ISegment> segments, int notBefore)
    {
      if (notBefore < -1 || notBefore > segments.Count - 2)
        notBefore = -1;
      this._dynamicStartIndex = notBefore;
      TimeSpan timeSpan = TimeSpan.FromSeconds(30.0);
      for (int index = segments.Count - 1; index > notBefore && segments[index].Duration.HasValue; --index)
      {
        timeSpan -= segments[index].Duration.Value;
        if (timeSpan < TimeSpan.Zero)
        {
          this._dynamicStartIndex = Math.Max(notBefore, Math.Min(index - 1, segments.Count - 2));
          break;
        }
      }
      this._startSegmentIndex = this._dynamicStartIndex;
    }

    private void UpdateDynamicPlaylistExpiration(IList<ISegment> segments)
    {
      TimeSpan? nullable = this.GetDuration(segments, Math.Min(segments.Count - 1, Math.Max(this._startSegmentIndex + 2, segments.Count - 4)), segments.Count);
      if (!nullable.HasValue)
      {
        int num = segments.Count - this._startSegmentIndex;
        nullable = num <= 0 ? new TimeSpan?(TimeSpan.Zero) : new TimeSpan?(new TimeSpan(0, 0, 0, 3 * num));
      }
      TimeSpan timeSpan = nullable.Value;
      if (timeSpan < this._minimumReload)
        timeSpan = this._minimumReload;
      else if (timeSpan > this._maximumReload)
        timeSpan = this._maximumReload;
      this._segmentsExpiration = Environment.TickCount + (int) timeSpan.TotalMilliseconds;
    }

    private TimeSpan? GetDuration(IEnumerable<ISegment> segments)
    {
      TimeSpan timeSpan = TimeSpan.Zero;
      foreach (ISegment segment in segments)
      {
        TimeSpan? saneDuration = this.GetSaneDuration(segment.Duration);
        if (!saneDuration.HasValue)
          return new TimeSpan?();
        timeSpan += saneDuration.Value;
      }
      return new TimeSpan?(timeSpan);
    }

    private TimeSpan? GetSaneDuration(TimeSpan? duration)
    {
      if (!duration.HasValue)
        return new TimeSpan?();
      if (duration.Value <= TimeSpan.Zero || duration.Value >= this._excessiveDuration)
        return new TimeSpan?();
      return duration;
    }

    private TimeSpan? GetDuration(IList<ISegment> segments, int start, int end)
    {
      bool flag = true;
      TimeSpan timeSpan1 = TimeSpan.Zero;
      for (int index = start; index < end; ++index)
      {
        ISegment segment = segments[index];
        TimeSpan? nullable = segment.Duration;
        if (!nullable.HasValue)
        {
          nullable = new TimeSpan?();
          return nullable;
        }
        nullable = segment.Duration;
        TimeSpan timeSpan2 = TimeSpan.Zero;
        int num;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() <= timeSpan2 ? 1 : 0) : 0) == 0)
        {
          nullable = segment.Duration;
          TimeSpan timeSpan3 = this._excessiveDuration;
          num = (nullable.HasValue ? (nullable.GetValueOrDefault() > timeSpan3 ? 1 : 0) : 0) == 0 ? 1 : 0;
        }
        else
          num = 0;
        if (num == 0)
        {
          nullable = new TimeSpan?();
          return nullable;
        }
        nullable = segment.Duration;
        TimeSpan timeSpan4 = nullable.Value;
        if (flag)
        {
          flag = false;
          timeSpan4 = new TimeSpan(timeSpan4.Ticks / 2L);
        }
        timeSpan1 += timeSpan4;
      }
      return new TimeSpan?(timeSpan1);
    }

    private class PlaylistEnumerable : IAsyncEnumerable<ISegment>
    {
      private readonly HlsPlaylistSegmentManager _segmentManager;

      public PlaylistEnumerable(HlsPlaylistSegmentManager segmentManager)
      {
        if (null == segmentManager)
          throw new ArgumentNullException("segmentManager");
        this._segmentManager = segmentManager;
      }

      public IAsyncEnumerator<ISegment> GetEnumerator()
      {
        return (IAsyncEnumerator<ISegment>) new HlsPlaylistSegmentManager.PlaylistEnumerator(this._segmentManager);
      }
    }

    private class PlaylistEnumerator : IAsyncEnumerator<ISegment>, IDisposable
    {
      private int _index = -1;
      private readonly HlsPlaylistSegmentManager _segmentManager;
      private ISegment[] _segments;

      public ISegment Current { get; private set; }

      public PlaylistEnumerator(HlsPlaylistSegmentManager segmentManager)
      {
        if (null == segmentManager)
          throw new ArgumentNullException("segmentManager");
        this._segmentManager = segmentManager;
      }

      public void Dispose()
      {
      }

      public async Task<bool> MoveNextAsync()
      {
        ISegment[] segments;
        while (true)
        {
          ConfiguredTaskAwaitable configuredTaskAwaitable = this._segmentManager.CheckReload(this._index).ConfigureAwait(false);
          await configuredTaskAwaitable;
          bool lockTaken = false;
          bool isDynamicPlaylist;
          int startIndex;
          object obj = null;
          try
          {
            Monitor.Enter(obj = this._segmentManager._segmentLock, ref lockTaken);
            isDynamicPlaylist = this._segmentManager._isDynamicPlaylist;
            segments = this._segmentManager._segments;
            startIndex = this._segmentManager._startSegmentIndex;
          }
          finally
          {
            if (lockTaken)
              Monitor.Exit(obj);
          }
          if (null != segments)
          {
            if (!object.ReferenceEquals((object) segments, (object) this._segments))
            {
              if (null != this._segments)
                this._index = HlsPlaylistSegmentManager.PlaylistEnumerator.FindNewIndex(this._segments, segments, this._index);
              else if (-1 == this._index)
                this._index = startIndex;
              this._segments = segments;
            }
            if (this._index + 1 >= segments.Length)
            {
              if (isDynamicPlaylist)
              {
                int delay = 5000;
                if (segments != null && 0 < segments.Length)
                {
                  ISegment segment = segments[segments.Length - 1];
                  TimeSpan? duration = segment.Duration;
                  if (duration.HasValue)
                  {
                    duration = segment.Duration;
                    delay = (int) (duration.Value.TotalMilliseconds / 2.0);
                  }
                }
                configuredTaskAwaitable = TaskEx.Delay(delay, this._segmentManager.CancellationToken).ConfigureAwait(false);
                await configuredTaskAwaitable;
              }
              else
                goto label_17;
            }
            else
              goto label_15;
          }
          else
            break;
        }
        bool flag = false;
        goto label_25;
label_15:
        this.Current = segments[++this._index];
        flag = true;
        goto label_25;
label_17:
        flag = false;
label_25:
        return flag;
      }

      private static int FindNewIndex(ISegment[] oldSegments, ISegment[] newSegments, int oldIndex)
      {
        Debug.Assert(null != newSegments);
        if (oldSegments == null || oldIndex < 0 || oldIndex >= oldSegments.Length || newSegments.Length < 1)
          return -1;
        ISegment segment = oldSegments[oldIndex];
        long? mediaSequence = segment.MediaSequence;
        if (mediaSequence.HasValue)
        {
          int indexByMediaSequence = HlsPlaylistSegmentManager.PlaylistEnumerator.FindIndexByMediaSequence(mediaSequence.Value, (IList<ISegment>) newSegments);
          if (indexByMediaSequence >= 0)
            return indexByMediaSequence;
        }
        Uri url = segment.Url;
        int indexByUrl = HlsPlaylistSegmentManager.PlaylistEnumerator.FindIndexByUrl(url, (IList<ISegment>) newSegments);
        if (indexByUrl >= 0)
          return indexByUrl;
        Debug.WriteLine("HlsPlaylistSegmentManager.FindNewIndex(): playlist discontinuity, does not contain {0}", (object) url);
        return -1;
      }

      private static int FindIndexByUrl(Uri url, IList<ISegment> segments)
      {
        for (int index = 0; index < segments.Count; ++index)
        {
          if (url == segments[index].Url)
            return index;
        }
        return -1;
      }

      private static int FindIndexByMediaSequence(long mediaSequence, IList<ISegment> segments)
      {
        long? mediaSequence1 = segments[0].MediaSequence;
        if (!mediaSequence1.HasValue || mediaSequence < mediaSequence1.Value)
          return -1;
        int index = (int) (mediaSequence - mediaSequence1.Value);
        if (index >= segments.Count)
          return -1;
        long? mediaSequence2 = segments[index].MediaSequence;
        long num = mediaSequence;
        if ((mediaSequence2.GetValueOrDefault() != num ? 0 : (mediaSequence2.HasValue ? 1 : 0)) != 0)
          return index;
        return -1;
      }
    }
  }
}
