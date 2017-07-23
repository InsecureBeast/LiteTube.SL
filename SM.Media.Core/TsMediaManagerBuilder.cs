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
             new SmMediaModule(),
             new TsParserModule(),
             new HlsModule(),
             new TsMediaModule()
        };

        public TsMediaManagerBuilder(bool useHttpConnection, bool useSingleStreamMediaManager, VideoQuality quality)
            : base(Modules)
        {
            RegistrationExtensions.Register(ContainerBuilder, (q) => quality);

            if (useHttpConnection)
                BuilderBaseExtensions.RegisterModule<HttpConnectionModule>(this);
            else
                BuilderBaseExtensions.RegisterModule<HttpClientModule>(this);
            if (useSingleStreamMediaManager)
                BuilderBaseExtensions.RegisterModule<SingleStreamMediaManagerModule>(this);
            else
                BuilderBaseExtensions.RegisterModule<SmMediaManagerModule>(this);

            RegistrationExtensions.Register(ContainerBuilder, _ => ApplicationInformationFactory.Default).SingleInstance();
        }
    }
}
