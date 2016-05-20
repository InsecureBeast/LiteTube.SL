using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteTube.DataClasses;

namespace LiteTube.ViewModels.Nodes
{
    class PlaylistNodeViewModel : NodeViewModelBase
    {
        private string _id;

        public PlaylistNodeViewModel(IPlaylist item)
        {
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
    }
}
