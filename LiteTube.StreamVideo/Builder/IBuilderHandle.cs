using System;

namespace LiteTube.StreamVideo.Builder
{
    public interface IBuilderHandle<TBuild> : IDisposable
    {
        TBuild Instance { get; }
    }
}
