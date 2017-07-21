using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Web
{
  public class WebCacheManager : IWebCacheManager, IDisposable
  {
    private readonly Dictionary<Uri, WebCacheManager.CacheEntry> _cache = new Dictionary<Uri, WebCacheManager.CacheEntry>();
    private CancellationTokenSource _flushCancellationTokenSource = new CancellationTokenSource();
    private readonly IWebReader _webReader;

    public WebCacheManager(IWebReader webReader)
    {
      if (null == webReader)
        throw new ArgumentNullException("webReader");
      this._webReader = webReader;
    }

    public async Task FlushAsync()
    {
      this._flushCancellationTokenSource.Cancel();
      bool lockTaken = false;
      WebCacheManager.CacheEntry[] cacheEntries;
      Dictionary<Uri, WebCacheManager.CacheEntry> dictionary = null;
      try
      {
        Monitor.Enter((object) (dictionary = this._cache), ref lockTaken);
        cacheEntries = Enumerable.ToArray<WebCacheManager.CacheEntry>((IEnumerable<WebCacheManager.CacheEntry>) this._cache.Values);
        this._cache.Clear();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      try
      {
        await TaskEx.WhenAll(Enumerable.Select<WebCacheManager.CacheEntry, Task>(Enumerable.Where<WebCacheManager.CacheEntry>((IEnumerable<WebCacheManager.CacheEntry>) cacheEntries, (Func<WebCacheManager.CacheEntry, bool>) (c => null != c.ReadTask)), (Func<WebCacheManager.CacheEntry, Task>) (c => c.ReadTask))).ConfigureAwait(false);
      }
      catch (OperationCanceledException ex)
      {
      }
      catch (Exception ex)
      {
        Debug.WriteLine("WebCacheManager.FlushAsync() exception: " + ExceptionExtensions.ExtendedMessage(ex));
      }
      foreach (WebCacheManager.CacheEntry cacheEntry in cacheEntries)
        cacheEntry.WebCache.WebReader.Dispose();

            CancellationTokenSource fcts = this._flushCancellationTokenSource;
      this._flushCancellationTokenSource = new CancellationTokenSource();
      fcts.Dispose();
    }

    public Task<TCached> ReadAsync<TCached>(Uri uri, Func<Uri, byte[], TCached> factory, ContentKind contentKind, ContentType contentType, CancellationToken cancellationToken) where TCached : class
    {
      TaskCompletionSource<TCached> tcs = (TaskCompletionSource<TCached>) null;
      bool lockTaken = false;
      Dictionary<Uri, WebCacheManager.CacheEntry> dictionary = null;
      WebCacheManager.CacheEntry cacheEntry;
      try
      {
        Monitor.Enter((object) (dictionary = this._cache), ref lockTaken);
        if (this._cache.TryGetValue(uri, out cacheEntry))
        {
          if (cacheEntry.ReadTask.IsCompleted && cacheEntry.Age > TimeSpan.FromSeconds(5.0))
          {
            tcs = new TaskCompletionSource<TCached>();
            cacheEntry.ReadTask = (Task) tcs.Task;
          }
        }
        else
        {
          IWebCache webCache = WebReaderExtensions.CreateWebCache(this._webReader, uri, contentKind, contentType);
          tcs = new TaskCompletionSource<TCached>();
          cacheEntry = new WebCacheManager.CacheEntry()
          {
            WebCache = webCache,
            ReadTask = (Task) tcs.Task
          };
          this._cache[uri] = cacheEntry;
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) dictionary);
      }
      if (null == tcs)
        return (Task<TCached>) cacheEntry.ReadTask;
      Task<TCached> task = cacheEntry.WebCache.ReadAsync<TCached>(factory, cancellationToken, (WebResponse) null);
      task.ContinueWith((Action<Task<TCached>>) (t =>
      {
        cacheEntry.ResetTime();
        if (null != t.Exception)
          tcs.TrySetCanceled();
        else if (t.IsCanceled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult(task.Result);
      }), cancellationToken);
      return tcs.Task;
    }

    public void Dispose()
    {
      this._webReader.Dispose();
    }

    private class CacheEntry
    {
      private Stopwatch _lastUpdate;
      public Task ReadTask;
      public IWebCache WebCache;

      public TimeSpan Age
      {
        get
        {
          return this._lastUpdate.Elapsed;
        }
      }

      public void ResetTime()
      {
        this._lastUpdate = Stopwatch.StartNew();
      }
    }
  }
}
