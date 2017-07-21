using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
    public interface IAsyncEnumerator<T> : IDisposable
    {
        T Current { get; }

        Task<bool> MoveNextAsync();
    }
}
