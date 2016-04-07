using LiteTube.Common;
using LiteTube.DataClasses;
using System.Globalization;
using System.Windows.Input;
using System.Threading.Tasks;
using System;

namespace LiteTube.ViewModels.Nodes
{
    class PlayListItemNodeViewModel : NodeViewModelBase
    {
        private readonly string _videoId;
        private readonly string _id;
        private readonly Func<Task> _deleteFunc;
        private readonly RelayCommand _deleteCommand;
        private bool _isSelected;

        public PlayListItemNodeViewModel(IPlayListItem item, Func<Task> deleteFunc) : this(item)
        {
            _deleteFunc = deleteFunc;
        }

        public PlayListItemNodeViewModel(IPlayListItem item)
        {
            PlayListItem = item;
            _id = item.Id;
            _videoId = item.ContentDetails.VideoId;
            Title = item.Snippet.Title;
            Description = item.Snippet.Description;
            ImagePath = item.Snippet.Thumbnails.GetThumbnailUrl();
            if (item.Snippet.PublishedAt != null)
                PublishedAt = item.Snippet.PublishedAt.Value.ToString("d", CultureInfo.CurrentCulture);

            Duration = null;
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
        public string PublishedAt { get; private set; }
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
            if (_deleteFunc == null)
                return;
            await _deleteFunc();
        }
    }
}
