using LiteTube.DataClasses;
using LiteTube.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.Common;

namespace LiteTube.ViewModels
{
    public class PlaylistVideosViewModel : SectionBaseViewModel
    {
        private readonly string _playlistId;
        private Action<NavigationObject> _itemTap;

        public PlaylistVideosViewModel(string playlistId, Func<IDataSource> geDataSource, IConnectionListener connectionListener, Action<NavigationObject> itemTap)
            : base(geDataSource, connectionListener)
        {
            if (playlistId == null)
                return;

            _playlistId = playlistId;
            _itemTap = itemTap;
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getGeDataSource().GetVideoPlaylist(_playlistId, nextPageToken);
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            _itemTap(navObject);
        }
    }
}
