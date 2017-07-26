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
        private static readonly IModule[] _modules = 
        {
             new SmMediaModule(),
             new TsParserModule(),
             new HlsModule(),
             new TsMediaModule()
        };

        public TsMediaManagerBuilder(bool useHttpConnection, bool useSingleStreamMediaManager, VideoQuality quality)
            : base(_modules)
        {
            ContainerBuilder.Register(q => quality);

            if (useHttpConnection)
                this.RegisterModule<HttpConnectionModule>();
            else
                this.RegisterModule<HttpClientModule>();
            if (useSingleStreamMediaManager)
                this.RegisterModule<SingleStreamMediaManagerModule>();
            else
                this.RegisterModule<SmMediaManagerModule>();

            ContainerBuilder.Register(_ => ApplicationInformationFactory.Default).SingleInstance();
        }
    }
}
