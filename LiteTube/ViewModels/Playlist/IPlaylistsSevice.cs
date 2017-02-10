using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.ViewModels.Playlist
{
    public interface IPlaylistsSevice
    {
        void ShowContainer(bool show, string videoId);
    }
}
