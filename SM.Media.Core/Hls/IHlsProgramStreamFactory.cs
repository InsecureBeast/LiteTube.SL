using System;
using System.Collections.Generic;
using SM.Media.Core.Web;

namespace SM.Media.Core.Hls
{
    public interface IHlsProgramStreamFactory
    {
        IHlsProgramStream Create(ICollection<Uri> urls, IWebReader webReader);
    }
}
