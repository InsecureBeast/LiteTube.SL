using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;

namespace LiteTube.Core.DataClasses
{
    public interface ICommentList : IResponceList
    {
        IEnumerable<IComment> Items { get; }
    }

    public interface IComment
    {
        string TextDisplay { get; }
        string AuthorDisplayName { get; }
        string AuthorProfileImageUrl { get; }
        string AuthorChannelId { get; }
        long? LikeCount { get; }
        DateTime? PublishedAt { get; }
        string PublishedAtRaw { get; }
        IEnumerable<IComment> ReplayComments { get; } 
        bool IsReplay { get; }
    }

    class MCommentList : ICommentList
    {
        public MCommentList(CommentThreadListResponse response)
        {
            if (response == null)
                return;

            NextPageToken = response.NextPageToken;
            PageInfo = new MPageInfo(response.PageInfo);
            VisitorId = response.VisitorId;
            Items = response.Items.Select(c => new MComment(c));
        }

        public static ICommentList EmptyList
        {
            get { return new MCommentList(); } 
        }

        private MCommentList()
        {
            Items = new List<IComment>();
        }

        public IEnumerable<IComment> Items
        {
            get;
            private set;
        }

        public string NextPageToken
        {
            get;
            private set;
        }

        public IPageInfo PageInfo
        {
            get;
            private set;
        }

        public string PrevPageToken
        {
            get;
            private set;
        }

        public string VisitorId
        {
            get;
            private set;
        }
    }

    class MComment : IComment
    {
        public static IComment Empty
        {
            get { return new MComment((Comment)null); }
        }
        
        
        public MComment(Comment comment)
        {
            if (comment == null)
                return;

            var authorSnippet = comment.Snippet;
            if (authorSnippet == null)
                return;

            if (!string.IsNullOrEmpty(authorSnippet.AuthorChannelUrl))
                AuthorChannelId = GetChannelId(authorSnippet.AuthorChannelUrl);

            AuthorDisplayName = authorSnippet.AuthorDisplayName;
            AuthorProfileImageUrl = authorSnippet.AuthorProfileImageUrl;
            LikeCount = authorSnippet.LikeCount;
            PublishedAt = authorSnippet.PublishedAt;
            if (PublishedAt != null)
                PublishedAtRaw = PublishedAt.Value.ToString("D");
            TextDisplay = authorSnippet.TextDisplay;
            IsReplay = true;
            ReplayComments = new List<IComment>();
        }

        public MComment(CommentThread comment)
        {
            if (comment == null)
                return;

            if (comment.Snippet == null)
                return;

            if (comment.Snippet.TopLevelComment == null)
                return;
            
            var authorSnippet = comment.Snippet.TopLevelComment.Snippet;
            if (authorSnippet == null)
                return;

            if (!string.IsNullOrEmpty(authorSnippet.AuthorChannelUrl))
                AuthorChannelId = GetChannelId(authorSnippet.AuthorChannelUrl);
            AuthorDisplayName = authorSnippet.AuthorDisplayName;
            AuthorProfileImageUrl = authorSnippet.AuthorProfileImageUrl;
            LikeCount = authorSnippet.LikeCount;
            PublishedAt = authorSnippet.PublishedAt;
            if (PublishedAt != null)
                PublishedAtRaw = PublishedAt.Value.ToString("g");
            TextDisplay = authorSnippet.TextDisplay;

            IsReplay = false;

            ReplayComments = new List<IComment>();
            if (comment.Replies == null) 
                return;
            
            if (comment.Replies.Comments != null)
                ReplayComments = comment.Replies.Comments.Select(c => new MComment(c));
        }

        public string AuthorChannelId
        {
            get;
            private set;
        }

        public string AuthorDisplayName
        {
            get;
            private set;
        }

        public string AuthorProfileImageUrl
        {
            get;
            private set;
        }

        public bool IsReplay
        {
            get;
            private set;
        }

        public long? LikeCount
        {
            get;
            private set;
        }

        public DateTime? PublishedAt
        {
            get;
            private set;
        }

        public string PublishedAtRaw
        {
            get;
            private set;
        }

        public IEnumerable<IComment> ReplayComments
        {
            get;
            private set;
        }

        public string TextDisplay
        {
            get;
            private set;
        }

        private string GetChannelId(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (!url.Contains("/"))
                return string.Empty;

            var startIndex = url.LastIndexOf("/") + 1;
            if (url.Length <= startIndex)
                return string.Empty;

            var result = url.Substring(startIndex);
            return result;
        }
    }
}
