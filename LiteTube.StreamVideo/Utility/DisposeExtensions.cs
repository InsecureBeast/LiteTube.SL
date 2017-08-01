using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
  public static class DisposeExtensions
  {
    public static void DisposeSafe(this IDisposable disposable)
    {
      if (null == disposable)
        return;
      try
      {
        disposable.Dispose();
      }
      catch (Exception ex)
      {
        Debug.WriteLine("DisposeExtensions.DisposeSafe() for {0} failed: {1}", (object) disposable.GetType().FullName, (object) ex.Message);
      }
    }

    public static Task DisposeAsync(this IDisposable disposable)
    {
      if (null == disposable)
        return TplTaskExtensions.CompletedTask;
      return TaskEx.Run((Action) (() => DisposeExtensions.DisposeSafe(disposable)));
    }

    public static void DisposeBackground(this IDisposable disposable, string description)
    {
      TaskCollector.Default.Add(DisposeExtensions.DisposeAsync(disposable), description);
    }
  }
}
