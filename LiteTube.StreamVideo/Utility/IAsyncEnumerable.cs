namespace LiteTube.StreamVideo.Utility
{
    public interface IAsyncEnumerable<T>
    {
        IAsyncEnumerator<T> GetEnumerator();
    }
}
