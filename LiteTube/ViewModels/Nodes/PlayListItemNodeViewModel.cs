using LiteTube.Common;
using LiteTube.DataClasses;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using LiteTube.DataModel;

namespace LiteTube.ViewModels.Nodes
{
    class PlayListItemNodeViewModel : NodeViewModelBase
    {
        private readonly string _videoId;
        private readonly string _id;
        private readonly RelayCommand _deleteCommand;
        private bool _isSelected;
        private readonly Func<Task> _delete;

        public PlayListItemNodeViewModel(IPlayListItem item, IDataSource dataSource, Func<Task> delete, IContextMenuStrategy menuProvider) : base(dataSource, menuProvider)
        {
            PlayListItem = item;
            _id = item.Id;
            _videoId = item.ContentDetails.VideoId;
            Title = item.Snippet.Title;
            Description = item.Snippet.Description;
            ImagePath = item.Snippet.Thumbnails.GetThumbnailUrl();
            PublishedAt = item.Snippet.PublishedAt;
            Duration = null;
            _delete = delete;
            _deleteCommand = new RelayCommand(Delete);
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public IPlayListItem PlayListItem { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public TimeSpan? Duration { get; private set; }

        public override string Id
        {
            get { return _id; }
        }

        public override string VideoId
        {
            get { return _videoId; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChanged(() => IsSelected);
            }
        }

        public override string ToString()
        {
            return Title;
        }

        private async void Delete()
        {
            IsSelected = true;
            if (_delete == null)
                return;
            await _delete();
        }
    }
}
