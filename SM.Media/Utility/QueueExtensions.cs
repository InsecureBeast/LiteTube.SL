// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.QueueExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace SM.Media.Utility
{
  public static class QueueExtensions
  {
    public static bool Remove<T>(this Queue<T> queue, T item) where T : class
    {
      if (!queue.Contains(item))
        return false;
      T[] objArray = queue.ToArray();
      queue.Clear();
      bool flag = false;
      foreach (T obj in objArray)
      {
        if (object.ReferenceEquals((object) obj, (object) item))
        {
          if (flag)
            Debug.WriteLine("QueueExtensions.Remove() multiple matches");
          flag = true;
        }
        else
          queue.Enqueue(obj);
      }
      return flag;
    }
  }
}
