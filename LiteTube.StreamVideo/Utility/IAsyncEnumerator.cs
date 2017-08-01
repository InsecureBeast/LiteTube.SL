using System;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
    public interface IAsyncEnumerator<T> : IDisposable
    {
        T Current { get; }

        Task<bool> MoveNextAsync();
    }
}
