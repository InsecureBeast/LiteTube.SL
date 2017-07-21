using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Builder
{
    public interface IBuilderHandle<TBuild> : IDisposable
    {
        TBuild Instance { get; }
    }
}
