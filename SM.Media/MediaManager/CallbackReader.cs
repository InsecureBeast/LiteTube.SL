// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaManager.CallbackReader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.Metadata;
using SM.Media.Segments;
using SM.Media.Utility;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.MediaManager
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
      this._segmentReaders = segmentReaders;
      this._enqueue = enqueue;
      this._bufferPool = bufferPool;
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      try
      {
        this.Close();
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }
      bool lockTaken = false;
      object obj;
      Task task;
      CancellationTokenSource cancellationTokenSource;
      try
      {
        Monitor.Enter(obj = this._readerLock, ref lockTaken);
        task = this._readerTask;
        this._readerTask = TplTaskExtensions.CompletedTask;
        cancellationTokenSource = this._readCancellationSource;
        this._readCancellationSource = (CancellationTokenSource) null;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (null != task)
        TaskCollector.Default.Add(task, "CallbackReader.Close");
      if (null == cancellationTokenSource)
        return;
      CancellationTokenExtensions.CancelDisposeSafe(cancellationTokenSource);
    }

    private void Close()
    {
      bool lockTaken = false;
      object obj;
      try
      {
        Monitor.Enter(obj = this._readerLock, ref lockTaken);
        this._isClosed = true;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      try
      {
        this.StopAsync().Wait();
      }
      catch (Exception ex)
      {
        Debug.WriteLine("CallbackReader.Close() failed: " + ExceptionExtensions.ExtendedMessage(ex));
      }
    }

    protected virtual async Task ReadSegmentsAsync(CancellationToken cancellationToken)
    {
      bool lockTaken = false;
      TaskCompletionSource<long> readResultTask;
      object obj;
      try
      {
        Monitor.Enter(obj = this._readerLock, ref lockTaken);
        readResultTask = this._readResultTask;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      this._total = 0L;
      try
      {
        using (IAsyncEnumerator<ISegmentReader> enumerator = this._segmentReaders.GetEnumerator())
        {
          while (true)
          {
            if (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
              ISegmentReader segmentReader = enumerator.Current;
              DateTimeOffset start = DateTimeOffset.Now;
              Debug.WriteLine("++++ Starting {0} at {1}.  Total memory: {2:F} MiB", (object) segmentReader, (object) start, (object) ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
              await this.ReadSegmentAsync(segmentReader, cancellationToken).ConfigureAwait(false);
              DateTimeOffset complete = DateTimeOffset.Now;
              Debug.WriteLine("---- Completed {0} at {1} ({2}).  Total memory: {3:F} MiB", (object) segmentReader, (object) complete, (object) (complete - start), (object) ByteConversion.BytesToMiB(GC.GetTotalMemory(false)));
            }
            else
              break;
          }
        }
        this._enqueue((WorkBuffer) null);
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
          readResultTask.TrySetResult(this._total);
      }
    }

    protected virtual async Task ReadSegmentAsync(ISegmentReader segmentReader, CancellationToken cancellationToken)
    {
      WorkBuffer buffer = (WorkBuffer) null;
      try
      {
        while (!segmentReader.IsEof)
        {
          if (null == buffer)
            buffer = await this._bufferPool.AllocateAsync(cancellationToken).ConfigureAwait(false);
          Debug.Assert(null != buffer);
          WorkBuffer localBuffer = buffer;
          int length = await segmentReader.ReadAsync(buffer.Buffer, 0, buffer.Buffer.Length, (Action<ISegmentMetadata>) (metadata => localBuffer.Metadata = metadata), cancellationToken).ConfigureAwait(false);
          buffer.Length = length;
          buffer.ReadCount = ++this._readCount;
          if (buffer.Length > 0)
          {
            this._total += (long) buffer.Length;
            this._enqueue(buffer);
            buffer = (WorkBuffer) null;
          }
        }
      }
      finally
      {
        if (null != buffer)
          this._bufferPool.Free(buffer);
      }
    }

    public virtual Task<long> ReadAsync(CancellationToken cancellationToken)
    {
      CancellationTokenSource cancellationTokenSource = (CancellationTokenSource) null;
      TaskCompletionSource<long> completionSource1 = (TaskCompletionSource<long>) null;
      bool lockTaken = false;
      object obj;
      TaskCompletionSource<long> completionSource2;
      try
      {
        Monitor.Enter(obj = this._readerLock, ref lockTaken);
        Debug.Assert(this._readerTask == null || this._readerTask.IsCompleted);
        if (this._isClosed)
          return TaskEx.FromResult<long>(0L);
        if (this._readCancellationSource == null || this._readCancellationSource.IsCancellationRequested)
        {
          cancellationTokenSource = this._readCancellationSource;
          this._readCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }
        if (this._readResultTask.Task.IsCompleted)
        {
          completionSource1 = this._readResultTask;
          this._readResultTask = new TaskCompletionSource<long>();
        }
        completionSource2 = this._readResultTask;
        CancellationToken token = this._readCancellationSource.Token;
        this._readerTask = TaskEx.Run((Func<Task>) (() => this.ReadSegmentsAsync(token)), token);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (null != completionSource1)
        completionSource1.TrySetCanceled();
      if (null != cancellationTokenSource)
        CancellationTokenExtensions.CancelDisposeSafe(cancellationTokenSource);
      return completionSource2.Task;
    }

    public virtual async Task StopAsync()
    {
      bool lockTaken = false;
      Task reader;
      CancellationTokenSource cancellationTokenSource;
      TaskCompletionSource<long> readResultTask;
      object obj;
      try
      {
        Monitor.Enter(obj = this._readerLock, ref lockTaken);
        reader = this._readerTask;
        readResultTask = this._readResultTask;
        cancellationTokenSource = this._readCancellationSource;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
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
