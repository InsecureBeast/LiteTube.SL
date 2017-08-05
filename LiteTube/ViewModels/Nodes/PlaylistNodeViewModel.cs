using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.Common;

namespace LiteTube.ViewModels.Nodes
{
    class PlaylistNodeViewModel : NodeViewModelBase
    {
        private readonly DeleteDelegate _delete;
        private readonly RelayCommand _deleteCommand;
        private readonly string _id;
        
        public delegate Task DeleteDelegate(string playlistId);

        public PlaylistNodeViewModel(IPlaylist item, IDataSource dataSource, IContextMenuStrategy menu, DeleteDelegate delete, bool isLargeItems)
            : base(dataSource, menu, isLargeItems)
        {
            _delete = delete;
            _deleteCommand = new RelayCommand(Delete);

            _id = item.Id;
            if (item.Snippet != null)
            {
                Title = item.Snippet.Title;
                ImagePath = item.Snippet.Thumbnails.GetThumbnailUrl();
                PublishedAt = item.Snippet.PublishedAt;
            }
            if (item.ContentDetails == null)
                return;

            ItemsCount = item.ContentDetails.ItemCount;
        }

        public override string Id
        {
            get { return _id; }
        }

        public override string VideoId
        {
            get { return string.Empty; }
        }

        public string Title { get; private set; }
        public string ImagePath { get; private set; }
        public long? ItemsCount { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public bool IsLive { get { return false; } }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        private async void Delete()
        {
            if (_delete == null)
                return;
            await _delete(_id);
        }
    }
}
