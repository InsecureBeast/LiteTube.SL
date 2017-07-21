using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using SM.Media.Core.Builder;
using SM.Media.Core.MediaManager;
using SM.Media.Core.Module;
using SM.Media.Core.Utility;

namespace SM.Media.Core
{
    public sealed class TsMediaManagerBuilder : BuilderBase<IMediaManager>
    {
        private static readonly IModule[] Modules = new IModule[4]
        {
            (IModule) new SmMediaModule(),
            (IModule) new TsParserModule(),
            (IModule) new HlsModule(),
            (IModule) new TsMediaModule()
        };

        public TsMediaManagerBuilder(bool useHttpConnection, bool useSingleStreamMediaManager)
            : base(TsMediaManagerBuilder.Modules)
        {
            if (useHttpConnection)
                BuilderBaseExtensions.RegisterModule<HttpConnectionModule>((BuilderBase) this);
            else
                BuilderBaseExtensions.RegisterModule<HttpClientModule>((BuilderBase) this);
            if (useSingleStreamMediaManager)
                BuilderBaseExtensions.RegisterModule<SingleStreamMediaManagerModule>((BuilderBase) this);
            else
                BuilderBaseExtensions.RegisterModule<SmMediaManagerModule>((BuilderBase) this);
            RegistrationExtensions.Register<IApplicationInformation>(this.ContainerBuilder,
                (Func<IComponentContext, IApplicationInformation>) (_ => ApplicationInformationFactory.Default))
                .SingleInstance();
        }
    }
}
