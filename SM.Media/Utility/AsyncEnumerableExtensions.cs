// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.AsyncEnumerableExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Threading.Tasks;

namespace SM.Media.Utility
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
