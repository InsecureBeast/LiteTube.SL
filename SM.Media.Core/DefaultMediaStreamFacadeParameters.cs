using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM.Media.Core.Builder;
using SM.Media.Core.MediaManager;

namespace SM.Media.Core
{
    public static class DefaultMediaStreamFacadeParameters
    {
        public static Func<IMediaStreamFacadeBase> Factory = (Func<IMediaStreamFacadeBase>)(() => (IMediaStreamFacadeBase)new MediaStreamFacade((IBuilder<IMediaManager>)null));

        public static IMediaStreamFacade Create(this MediaStreamFacadeParameters parameters)
        {
            return (IMediaStreamFacade)(parameters.Factory ?? DefaultMediaStreamFacadeParameters.Factory)();
        }
    }
}
