using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Program;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Hls
{
    public sealed class HlsPlaylistSegmentManager : ISegmentManager
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
                lock (_segmentLock)
                { 
                    return _abortTokenSource?.Token ?? CancellationToken.None;
                }
            }
        }

        public IWebReader WebReader { get; private set; }

        public TimeSpan StartPosition { get; private set; }

        public TimeSpan? Duration { get; private set; }

        public ContentType ContentType { get; private set; }

        public IAsyncEnumerable<ISegment> Playlist { get; private set; }

        public IStreamMetadata StreamMetadata => _programStream.StreamMetadata;

        public HlsPlaylistSegmentManager(IProgramStream programStream, IPlatformServices platformServices, CancellationToken cancellationToken)
        {
            if (null == programStream)
                throw new ArgumentNullException("programStream");
            if (null == platformServices)
                throw new ArgumentNullException("platformServices");
            _programStream = programStream;
            _platformServices = platformServices;
            _cancellationToken = cancellationToken;
            var parameters = HlsPlaylistSettings.Parameters;
            _minimumRetry = parameters.MinimumRetry;
            _minimumReload = parameters.MinimumReload;
            _maximumReload = parameters.MaximumReload;
            _excessiveDuration = parameters.ExcessiveDuration;
            _abortTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
            _isDynamicPlaylist = true;
            _readTask = new SignalTask(ReadSubList, _abortTokenSource.Token);
            _segmentsExpiration = Environment.TickCount;
            Playlist = new PlaylistEnumerable(this);
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref _isDisposed, 1))
                return;
            _refreshTimer.Cancel();
            try
            {
                CleanupReader().Wait(_cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HlsPlaylistSegmentManager.Dispose() failed: " + ex.Message);
            }
            _refreshTimer.Dispose();
        }

        public async Task StartAsync()
        {
            ThrowIfDisposed();
            _cancellationToken.ThrowIfCancellationRequested();
            SignalTask oldReadTask = null;
            CancellationTokenSource cancellationTokenSource = null;
            lock (_segmentLock)
            {
                if (!_isRunning)
                {
                    if (_abortTokenSource.IsCancellationRequested)
                    {
                        cancellationTokenSource = _abortTokenSource;
                        _abortTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
                        oldReadTask = _readTask;
                        _readTask = new SignalTask(ReadSubList, _abortTokenSource.Token);
                    }
                    _isRunning = true;
                }
                else
                    return;
            }

            await CleanupReader(oldReadTask, cancellationTokenSource).ConfigureAwait(false);
            if (_programStream.Segments != null && _programStream.Segments.Count > 0)
                UpdatePlaylist();
            var segment = await Playlist.FirstOrDefaultAsync().ConfigureAwait(false);
            if (null == segment)
            {
                Debug.WriteLine("HlsPlaylistSegmentManager.StartAsync() no segments found");
                throw new FileNotFoundException("Unable to find the first segment");
            }
            ContentType = await _programStream.GetContentTypeAsync(_cancellationToken).ConfigureAwait(false);
            WebReader = _programStream.WebReader.CreateChild(null, ContentKind.AnyMedia, ContentType);
        }

        public Task StopAsync()
        {
            if (0 != _isDisposed)
                return TplTaskExtensions.CompletedTask;
            _refreshTimer.Cancel();

            CancellationTokenSource cancellationTokenSource;
            SignalTask signalTask;
            lock(_segmentLock)
            {
                _isRunning = false;
                cancellationTokenSource = _abortTokenSource;
                signalTask = _readTask;
            }
            cancellationTokenSource.CancelSafe();
            return signalTask.WaitAsync();
        }

        public async Task<TimeSpan> SeekAsync(TimeSpan timestamp)
        {
            ThrowIfDisposed();
            CancellationToken.ThrowIfCancellationRequested();
            if (_isDynamicPlaylist)
                await CheckReload(-1).ConfigureAwait(false);

            TimeSpan actualPosition;
            lock(_segmentLock)
            {
                Seek(timestamp);
                actualPosition = StartPosition;
            }
            return actualPosition;
        }

        private async Task CleanupReader()
        {
            SignalTask readTask;
            CancellationTokenSource cancellationTokenSource;
            lock(_segmentLock)
            {
                readTask = _readTask;
                cancellationTokenSource = _abortTokenSource;
                _abortTokenSource = null;
            }
            
            await CleanupReader(readTask, cancellationTokenSource).ConfigureAwait(false);
            lock(_segmentLock)
            {
                _readTask = null;
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
            if (0 != _isDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void Seek(TimeSpan timestamp)
        {
            StartPosition = TimeSpan.Zero;
            _startSegmentIndex = _isDynamicPlaylist ? _dynamicStartIndex : -1;
            if (_segments == null || _segments.Length < 1 || timestamp <= TimeSpan.Zero)
                return;
            TimeSpan timeSpan = TimeSpan.Zero;
            for (int index = 0; index < _segments.Length; ++index)
            {
                TimeSpan? saneDuration = GetSaneDuration(_segments[index].Duration);
                if (saneDuration.HasValue)
                {
                    if (timeSpan + saneDuration.Value > timestamp)
                    {
                        StartPosition = timeSpan;
                        _startSegmentIndex = index - 1;
                        return;
                    }
                    timeSpan += saneDuration.Value;
                }
                else
                    break;
            }
            StartPosition = timeSpan;
            _startSegmentIndex = _segments.Length;
        }

        private Task CheckReload(int index)
        {
            CancellationToken.ThrowIfCancellationRequested();
            lock(_segmentLock)
            {
                if (_isInitialized && (!_isDynamicPlaylist || !_isRunning || _segmentsExpiration - Environment.TickCount > 0))
                    return TplTaskExtensions.CompletedTask;

                _readTask.Fire();
                if (_segments == null || index + 1 >= _segments.Length)
                    return _readTask.WaitAsync();
            }
            return TplTaskExtensions.CompletedTask;
        }

        private async Task ReadSubList()
        {
            Debug.WriteLine("HlsPlaylistSegmentManager.ReadSubList({0})", DateTimeOffset.Now);
            try
            {
                _refreshTimer.Cancel();
                DateTime start = DateTime.UtcNow;
                await _programStream.RefreshPlaylistAsync(_cancellationToken).ConfigureAwait(false);
                TimeSpan fetchElapsed = DateTime.UtcNow - start;
                Debug.WriteLine("HlsPlaylistSegmentManager.ReadSubList() refreshed playlist in " + fetchElapsed);
                if (UpdatePlaylist())
                {
                    _readSubListFailureCount = 0;
                    if (!_isDynamicPlaylist)
                        return;

                    TimeSpan expiration = TimeSpan.FromMilliseconds(_segmentsExpiration - Environment.TickCount);
                    if (expiration < _minimumRetry)
                    {
                        Debug.WriteLine("Expiration too short: " + expiration);
                        expiration = _minimumRetry;
                    }
                    CancellationToken.ThrowIfCancellationRequested();
                    _refreshTimer.SetTimer(() => _readTask.Fire(), expiration);
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HlsPlaylistSegmentManager.ReadSubList() failed: " + ex.Message);
            }
            if (++_readSubListFailureCount > 3)
            {
                lock(_segmentLock)
                {
                    _segments = null;
                    _isDynamicPlaylist = false;
                }
            }
            else
            {
                var delay = 1.0 + (1 << 2 * _readSubListFailureCount);
                delay += delay / 2.0 * (_platformServices.GetRandomNumber() - 0.5);
                var timeSpan = TimeSpan.FromSeconds(delay);
                Debug.WriteLine("HlsPlaylistSegmentManager.ReadSubList(): retrying update in " + timeSpan);
                CancellationToken.ThrowIfCancellationRequested();
                _refreshTimer.SetTimer(() => _readTask.Fire(), timeSpan);
            }
        }

        private bool UpdatePlaylist()
        {
            var segments = _programStream.Segments.ToArray();
            var isDynamicPlaylist = _programStream.IsDynamicPlaylist;
            Duration = isDynamicPlaylist ? new TimeSpan?() : GetDuration(segments);
            lock(_segmentLock)
            {
                if (!_isRunning)
                    return true;
                UnlockedUpdatePlaylist(isDynamicPlaylist, segments);
                _isDynamicPlaylist = isDynamicPlaylist;
                _isInitialized = true;
            }
            
            Debug.WriteLine("HlsPlaylistSegmentManager.UpdatePlaylist: playlist {0} loaded with {1} entries. index: {2} dynamic: {3} expires: {4} ({5})", 
                _programStream, _segments.Length, _startSegmentIndex, isDynamicPlaylist, isDynamicPlaylist ? TimeSpan.FromMilliseconds(_segmentsExpiration - Environment.TickCount) : TimeSpan.Zero, DateTimeOffset.Now);
            return true;
        }

        private void UnlockedUpdatePlaylist(bool isDynamicPlaylist, ISegment[] segments)
        {
            var flag = false;
            if (isDynamicPlaylist || _dynamicPlaylists.Count > 0)
            {
                var segmentArray = _dynamicPlaylists.LastOrDefault();
                if (segments.Length > 0)
                {
                    if (segmentArray != null && SegmentsMatch(segmentArray[0], segments[0]) && segmentArray.Length == segments.Length)
                    {
                        Debug.WriteLine("HlsPlaylistSegmentManager.UpdatePlaylist(): need reload ({0})", (object)DateTimeOffset.Now);
                        int num = Environment.TickCount + (int)Math.Round(2.0 * _minimumRetry.TotalMilliseconds);
                        if (_segmentsExpiration < num)
                            _segmentsExpiration = num;
                        flag = true;
                    }
                    else
                    {
                        _dynamicPlaylists.Add(segments);
                        if (_dynamicPlaylists.Count > 4)
                            _dynamicPlaylists.RemoveAt(0);
                    }
                }
                if (!flag)
                {
                    segments = ResyncSegments();
                    if (isDynamicPlaylist)
                        UpdateDynamicPlaylistExpiration(segments);
                }
            }
            if (flag)
                return;
            _segments = segments;
        }

        private ISegment[] ResyncSegments()
        {
            ISegment[] segmentArray1 = null;
            int num = -1;
            foreach (var segmentArray2 in _dynamicPlaylists)
            {
                num = segmentArray2.Length;
                if (null == segmentArray1)
                {
                    _segmentList.AddRange(segmentArray2);
                    segmentArray1 = segmentArray2;
                }
                else
                {
                    bool flag = false;
                    for (int index1 = 0; index1 < segmentArray1.Length; ++index1)
                    {
                        if (SegmentsMatch(segmentArray1[index1], segmentArray2[0]))
                        {
                            for (int index2 = 1; index2 < segmentArray2.Length; ++index2)
                            {
                                if (index1 + index2 >= segmentArray1.Length || !SegmentsMatch(segmentArray1[index1 + index2], segmentArray2[index2]))
                                    _segmentList.Add(segmentArray2[index2]);
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        _segmentList.AddRange(segmentArray2);
                    segmentArray1 = segmentArray2;
                }
            }
            ISegment[] segmentArray3 = _segmentList.ToArray();
            _segmentList.Clear();
            SetDynamicStartIndex(segmentArray3, segmentArray3.Length - num - 1);
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
            _dynamicStartIndex = notBefore;
            TimeSpan timeSpan = TimeSpan.FromSeconds(30.0);
            for (int index = segments.Count - 1; index > notBefore && segments[index].Duration.HasValue; --index)
            {
                timeSpan -= segments[index].Duration.Value;
                if (timeSpan < TimeSpan.Zero)
                {
                    _dynamicStartIndex = Math.Max(notBefore, Math.Min(index - 1, segments.Count - 2));
                    break;
                }
            }
            _startSegmentIndex = _dynamicStartIndex;
        }

        private void UpdateDynamicPlaylistExpiration(IList<ISegment> segments)
        {
            TimeSpan? nullable = GetDuration(segments, Math.Min(segments.Count - 1, Math.Max(_startSegmentIndex + 2, segments.Count - 4)), segments.Count);
            if (!nullable.HasValue)
            {
                int num = segments.Count - _startSegmentIndex;
                nullable = num <= 0 ? TimeSpan.Zero : new TimeSpan(0, 0, 0, 3 * num);
            }
            TimeSpan timeSpan = nullable.Value;
            if (timeSpan < _minimumReload)
                timeSpan = _minimumReload;
            else if (timeSpan > _maximumReload)
                timeSpan = _maximumReload;
            _segmentsExpiration = Environment.TickCount + (int)timeSpan.TotalMilliseconds;
        }

        private TimeSpan? GetDuration(IEnumerable<ISegment> segments)
        {
            var timeSpan = TimeSpan.Zero;
            foreach (var saneDuration in segments.Select(segment => GetSaneDuration(segment.Duration)))
            {
                if (!saneDuration.HasValue)
                    return new TimeSpan?();
                timeSpan += saneDuration.Value;
            }
            return timeSpan;
        }

        private TimeSpan? GetSaneDuration(TimeSpan? duration)
        {
            if (!duration.HasValue)
                return new TimeSpan?();
            if (duration.Value <= TimeSpan.Zero || duration.Value >= _excessiveDuration)
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
                    TimeSpan timeSpan3 = _excessiveDuration;
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
            return timeSpan1;
        }

        private class PlaylistEnumerable : IAsyncEnumerable<ISegment>
        {
            private readonly HlsPlaylistSegmentManager _segmentManager;

            public PlaylistEnumerable(HlsPlaylistSegmentManager segmentManager)
            {
                if (null == segmentManager)
                    throw new ArgumentNullException(nameof(segmentManager));
                _segmentManager = segmentManager;
            }

            public IAsyncEnumerator<ISegment> GetEnumerator()
            {
                return new PlaylistEnumerator(_segmentManager);
            }
        }

        private class PlaylistEnumerator : IAsyncEnumerator<ISegment>
        {
            private int _index = -1;
            private readonly HlsPlaylistSegmentManager _segmentManager;
            private ISegment[] _segments;

            public ISegment Current { get; private set; }

            public PlaylistEnumerator(HlsPlaylistSegmentManager segmentManager)
            {
                if (null == segmentManager)
                    throw new ArgumentNullException(nameof(segmentManager));
                _segmentManager = segmentManager;
            }

            public void Dispose()
            {
            }

            public async Task<bool> MoveNextAsync()
            {
                ISegment[] segments;
                while (true)
                {
                    ConfiguredTaskAwaitable configuredTaskAwaitable = _segmentManager.CheckReload(_index).ConfigureAwait(false);
                    await configuredTaskAwaitable;
                    bool isDynamicPlaylist;
                    int startIndex;
                    lock(_segmentManager._segmentLock)
                    {
                        isDynamicPlaylist = _segmentManager._isDynamicPlaylist;
                        segments = _segmentManager._segments;
                        startIndex = _segmentManager._startSegmentIndex;
                    }
                    
                    if (null != segments)
                    {
                        if (!ReferenceEquals(segments, _segments))
                        {
                            if (null != _segments)
                                _index = FindNewIndex(_segments, segments, _index);
                            else if (-1 == _index)
                                _index = startIndex;
                            _segments = segments;
                        }
                        if (_index + 1 >= segments.Length)
                        {
                            if (isDynamicPlaylist)
                            {
                                int delay = 5000;
                                if (0 < segments.Length)
                                {
                                    ISegment segment = segments[segments.Length - 1];
                                    TimeSpan? duration = segment.Duration;
                                    if (duration.HasValue)
                                    {
                                        duration = segment.Duration;
                                        if (duration != null)
                                            delay = (int) (duration.Value.TotalMilliseconds/2.0);
                                    }
                                }
                                configuredTaskAwaitable = TaskEx.Delay(delay, _segmentManager.CancellationToken).ConfigureAwait(false);
                                await configuredTaskAwaitable;
                            }
                            else
                            {
                                return false;
                            }
                                
                        }
                        else
                        {
                            Current = segments[++_index];
                            return true;
                        }
                    }
                    else
                        break;
                }

                return false;
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
                    int indexByMediaSequence = FindIndexByMediaSequence(mediaSequence.Value, newSegments);
                    if (indexByMediaSequence >= 0)
                        return indexByMediaSequence;
                }
                Uri url = segment.Url;
                int indexByUrl = FindIndexByUrl(url, newSegments);
                if (indexByUrl >= 0)
                    return indexByUrl;
                Debug.WriteLine("HlsPlaylistSegmentManager.FindNewIndex(): playlist discontinuity, does not contain {0}", url);
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
                int index = (int)(mediaSequence - mediaSequence1.Value);
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
