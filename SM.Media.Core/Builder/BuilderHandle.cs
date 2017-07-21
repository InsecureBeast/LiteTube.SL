using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace SM.Media.Core.Builder
{
    public sealed class BuilderHandle<TBuild> : IBuilderHandle<TBuild>, IDisposable
    {
        private readonly ILifetimeScope _scope;
        private TBuild _instance;

        public TBuild Instance
        {
            get
            {
                if (object.Equals((object)default(TBuild), (object)this._instance))
                    this._instance = ResolutionExtensions.Resolve<TBuild>((IComponentContext)this._scope);
                return this._instance;
            }
        }

        public BuilderHandle(ILifetimeScope scope)
        {
            if (null == scope)
                throw new ArgumentNullException("scope");
            this._scope = scope;
        }

        public void Dispose()
        {
            using (this._scope)
                ;
        }
    }
}
