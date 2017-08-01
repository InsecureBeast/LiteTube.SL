using System.Collections.Generic;
using System.Linq;

namespace LiteTube.StreamVideo.Utility
{
  public static class EnumerableExtensions
  {
    public static T SingleOrDefaultSafe<T>(this IEnumerable<T> items)
    {
      if (null == items)
        return default (T);
      IList<T> list = items as IList<T>;
      if (null != list)
        return 1 == list.Count ? list[0] : default (T);
      using (IEnumerator<T> enumerator = items.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return default (T);
        T current = enumerator.Current;
        return enumerator.MoveNext() ? default (T) : current;
      }
    }

    public static bool SequencesAreEquivalent<T>(this IEnumerable<T> a, IEnumerable<T> b)
    {
      if (object.ReferenceEquals((object) a, (object) b))
        return true;
      if (a == null || null == b)
        return false;
      return Enumerable.SequenceEqual<T>(a, b);
    }
  }
}
