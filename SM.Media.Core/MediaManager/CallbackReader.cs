using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Metadata;
using SM.Media.Core.Segments;
using SM.Media.Core.Utility;

namespace SM.Media.Core.MediaManager
{
    public class CallbackReader : IDisposable
    {
        private readonly object _readerLock = new object();
        private TaskCompletionSource<long> _readResultTask = new TaskCompletionSource<long>();
        private readonly IBlockingPool<WorkBuffer> _bufferPool;
        private readonly Action<WorkBuffer> _enqueue;
        private readonly IAsyncEnumerable<ISegmentReader> _segmentReaders;
        private bool _isClosed;
        private int _isDisposed;
        private CancellationTokenSource _readCancellationSource;
        private Task _readerTask;
        private long _total;
        private int _readCount;

        public CallbackReader(IAsyncEnumerable<ISegmentReader> segmentReaders, Action<WorkBuffer> enqueue, IBlockingPool<WorkBuffer> bufferPool)
        {
            if (null == segmentReaders)
                throw new ArgumentNullException("segmentReaders");
            if (null == enqueue)
                throw new ArgumentNullException("enqueue");
            if (null == bufferPool)
                throw new ArgumentNullException("bufferPool");

            _segmentReaders = segmentReaders;
            _enqueue = enqueue;
            _bufferPool = bufferPool;
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref _isDisposed, 1))
                return;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Task task;
            CancellationTokenSource cancellationTokenSource;
            lock(_readerLock)
            {
                task = _readerTask;
                _readerTask = TplTaskExtensions.CompletedTask;
                cancellationTokenSource = _readCancellationSource;
                _readCancellationSource = null;
            }

            if (null != task)
                TaskCollector.Default.Add(task, "CallbackReader.Close");

            if (null == cancellationTokenSource)
                return;
            CancellationTokenExtensions.CancelDisposeSafe(cancellationTokenSource);
        }

        private void Close()
        {
            lock(_readerLock)
            {
                _isClosed = true;
            }
            try
            {
                StopAsync().Wait();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CallbackReader.Close() failed: " + ExceptionExtensions.ExtendedMessage(ex));
            }
        }

        protected virtual async Task ReadSegmentsAsync(CancellationToken cancellationToken)
        {
            TaskCompletionSource<long> readResultTask;
            lock(_readerLock)
            {
                readResultTask = _readResultTask;
            }
            
            _total = 0L;
            try
            {
                using (IAsyncEnumerator<ISegmentReader> enumerator = _segmentReaders.GetEnumerator())
                {
                    while (true)
                    {
                        if (await enumerator.MoveNextAsync().ConfigureAwait(false))
                        {
                            ISegmentReader segmentReader = enumerator.Current;
                            DateTimeOffset start = DateTimeOffset.Now;
                            Debug.WriteLine("++++ Starting {0} at {1}.  Total memory: {2:F} MiB", segmentReader, start, ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
                            await ReadSegmentAsync(segmentReader, cancellationToken).ConfigureAwait(false);
                            DateTimeOffset complete = DateTimeOffset.Now;
                            Debug.WriteLine("---- Completed {0} at {1} ({2}).  Total memory: {3:F} MiB", segmentReader, complete, (complete - start), ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
                        }
                        else
                            break;
                        }
                }
                _enqueue(null);
            }
            catch (OperationCanceledException ex)
            {
                readResultTask.TrySetCanceled();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CallbackReader.ReadAsync() failed: " + ExceptionExtensions.ExtendedMessage(ex));
                readResultTask.TrySetException(ex);
            }
            finally
            {
                if (!readResultTask.Task.IsCompleted)
                    readResultTask.TrySetResult(_total);
            }
        }

        protected virtual async Task ReadSegmentAsync(ISegmentReader segmentReader, CancellationToken cancellationToken)
        {
            WorkBuffer buffer = null;
            try
            {
                while (!segmentReader.IsEof)
                {
                    if (null == buffer)
                        buffer = await _bufferPool.AllocateAsync(cancellationToken).ConfigureAwait(false);

                    Debug.Assert(null != buffer);
                    WorkBuffer localBuffer = buffer;
                    int length = await segmentReader.ReadAsync(buffer.Buffer, 0, buffer.Buffer.Length, metadata => localBuffer.Metadata = metadata, cancellationToken).ConfigureAwait(false);
                    buffer.Length = length;
                    buffer.ReadCount = ++_readCount;
                    if (buffer.Length > 0)
                    {
                        _total += buffer.Length;
                        _enqueue(buffer);
                        buffer = null;
                    }
                }
            }
            finally
            {
            if (null != buffer)
                _bufferPool.Free(buffer);
            }
        }

        public virtual Task<long> ReadAsync(CancellationToken cancellationToken)
        {
            CancellationTokenSource cancellationTokenSource = null;
            TaskCompletionSource<long> completionSource1 = null;
            TaskCompletionSource<long> completionSource2;
            lock(_readerLock)
            {
                Debug.Assert(_readerTask == null || _readerTask.IsCompleted);
                if (_isClosed)
                    return TaskEx.FromResult(0L);

                if (_readCancellationSource == null || _readCancellationSource.IsCancellationRequested)
                {
                    cancellationTokenSource = _readCancellationSource;
                    _readCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                }

                if (_readResultTask.Task.IsCompleted)
                {
                    completionSource1 = _readResultTask;
                    _readResultTask = new TaskCompletionSource<long>();
                }

                completionSource2 = _readResultTask;
                CancellationToken token = _readCancellationSource.Token;
                _readerTask = TaskEx.Run(() => ReadSegmentsAsync(token), token);
            }
            
            if (null != completionSource1)
                completionSource1.TrySetCanceled();

            if (null != cancellationTokenSource)
                CancellationTokenExtensions.CancelDisposeSafe(cancellationTokenSource);

            return completionSource2.Task;
        }

        public virtual async Task StopAsync()
        {
            Task reader;
            CancellationTokenSource cancellationTokenSource;
            TaskCompletionSource<long> readResultTask;
            lock(_readerLock)
            {
                reader = _readerTask;
                readResultTask = _readResultTask;
                cancellationTokenSource = _readCancellationSource;
            }
            
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            cancellationTokenSource.Cancel();
            try
            {
                if (null != reader)
                    await reader.ConfigureAwait(false);

                if (null != readResultTask)
                {
                    long num = await readResultTask.Task.ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException ex)
            {
            }
        }
    }
}
