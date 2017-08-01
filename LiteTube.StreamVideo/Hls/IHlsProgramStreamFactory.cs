using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Hls
{
    public interface IHlsProgramStreamFactory
    {
        IHlsProgramStream Create(ICollection<Uri> urls, IWebReader webReader);
    }
}
