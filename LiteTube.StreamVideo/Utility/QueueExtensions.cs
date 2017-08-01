using System.Collections.Generic;
using System.Diagnostics;

namespace LiteTube.StreamVideo.Utility
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
