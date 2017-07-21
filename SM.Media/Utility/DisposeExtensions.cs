// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.DisposeExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SM.Media.Utility
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
