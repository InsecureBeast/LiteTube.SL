using LiteTube.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteTube.ViewModels.Nodes
{
    public class CommentNodeViewModel 
    {
        public CommentNodeViewModel(IComment comment)
        {
            TextDisplay = comment.TextDisplay;
            AuthorDisplayName = comment.AuthorDisplayName;
            AuthorProfileImageUrl = comment.AuthorProfileImageUrl;
            AuthorChannelId = comment.AuthorChannelId;
            LikeCount = comment.LikeCount;
            PublishedAt = comment.PublishedAt;
            PublishedAtRaw = comment.PublishedAtRaw;
            ReplayComments = comment.ReplayComments.Select(c => new CommentNodeViewModel(c));
            IsReplay = comment.IsReplay;
        }

        public string TextDisplay { get; private set; }
        public string AuthorDisplayName { get; private set; }
        public string AuthorProfileImageUrl { get; private set; }
        public string AuthorChannelId { get; private set; }
        public long? LikeCount { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public string PublishedAtRaw { get; private set; }
        public IEnumerable<CommentNodeViewModel> ReplayComments { get; private set; }
        public bool IsReplay { get; private set; }
    }
}
