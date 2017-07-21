using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM.Media.Core.Utility;

namespace SM.Media.Core
{
    public static class MediaStreamFacadeSettings
    {
        private static readonly ResettableParameters<MediaStreamFacadeParameters> MediaStreamFacadeParameters = new ResettableParameters<MediaStreamFacadeParameters>();

        public static MediaStreamFacadeParameters Parameters
        {
            get
            {
                return MediaStreamFacadeSettings.MediaStreamFacadeParameters.Parameters;
            }
            set
            {
                MediaStreamFacadeSettings.MediaStreamFacadeParameters.Parameters = value;
            }
        }
    }
}
