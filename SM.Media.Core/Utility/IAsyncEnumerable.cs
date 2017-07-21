using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
    public interface IAsyncEnumerable<T>
    {
        IAsyncEnumerator<T> GetEnumerator();
    }
}
