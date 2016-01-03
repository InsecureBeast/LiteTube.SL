using LiteTube.DataClasses;
using System.Globalization;

namespace LiteTube.ViewModels.Nodes
{
    class PlayListItemNodeViewModel : NodeViewModelBase
    {
        private string _videoId;
        private string _id;

        public PlayListItemNodeViewModel(IPlayListItem item)
        {
            PlayListItem = item;
            _id = item.Id;
            _videoId = item.ContentDetails.VideoId;
            Title = item.Snippet.Title;
            Description = item.Snippet.Description;
            ImagePath = item.Snippet.Thumbnails.Medium.Url;
            PublishedAt = item.Snippet.PublishedAt.Value.ToString("d", CultureInfo.CurrentCulture);
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

        public override string ToString()
        {
            return Title;
        }
    }
}
