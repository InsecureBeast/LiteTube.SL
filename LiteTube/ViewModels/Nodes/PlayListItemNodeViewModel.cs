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
        private string _videoId;
        private string _id;
        private readonly Func<Task> _deleteFunc;
        private bool _isSelected;
        private readonly RelayCommand _deleteCommand;

        public PlayListItemNodeViewModel(IPlayListItem item, Func<Task> deleteFunc)
        {
            PlayListItem = item;
            _id = item.Id;
            _videoId = item.ContentDetails.VideoId;
            Title = item.Snippet.Title;
            Description = item.Snippet.Description;
            ImagePath = item.Snippet.Thumbnails.Medium.Url;
            PublishedAt = item.Snippet.PublishedAt.Value.ToString("d", CultureInfo.CurrentCulture);

            _deleteCommand = new RelayCommand(Delete);
            _deleteFunc = deleteFunc;
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
            await _deleteFunc();
        }
    }
}
