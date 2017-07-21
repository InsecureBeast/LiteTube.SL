using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Builder
{
    public interface IBuilder<TBuild> : IBuilder, IDisposable
    {
        TBuild Create();

        void Destroy(TBuild instance);
    }

    public interface IBuilder : IDisposable
    {
        void Register<TService, TImplementation>() where TImplementation : TService;

        void RegisterSingleton<TService, TImplementation>() where TImplementation : TService;

        void RegisterSingleton<TService>(TService instance) where TService : class;

        void RegisterSingletonFactory<TService>(Func<TService> factory);

        void RegisterTransientFactory<TService>(Func<TService> factory);
    }
}
