using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  public static class AsyncEnumerableExtensions
  {
    public static async Task<T> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> source)
    {
      T obj;
      using (IAsyncEnumerator<T> enumerator = source.GetEnumerator())
        obj = await enumerator.MoveNextAsync().ConfigureAwait(false) ? enumerator.Current : default (T);
      return obj;
    }
  }
}
