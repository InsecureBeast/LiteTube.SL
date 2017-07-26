using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SM.Media.Core.Builder;
using SM.Media.Core.MediaManager;

namespace SM.Media.Core
{
    public class MediaStreamFacade : MediaStreamFacadeBase<MediaStreamSource>, IMediaStreamFacade
    {
        public MediaStreamFacade(VideoQuality quality, IBuilder<IMediaManager> builder = null) 
            : base(builder ?? new TsMediaManagerBuilder(MediaStreamFacadeSettings.Parameters.UseHttpConnection, MediaStreamFacadeSettings.Parameters.UseSingleStreamMediaManager, quality))
        {
        }
    }
}
