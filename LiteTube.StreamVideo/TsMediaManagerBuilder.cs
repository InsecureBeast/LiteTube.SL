using Autofac;
using Autofac.Core;
using LiteTube.StreamVideo.Builder;
using LiteTube.StreamVideo.MediaManager;
using LiteTube.StreamVideo.Module;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo
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
