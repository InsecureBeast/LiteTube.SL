using Google.Apis.YouTube.v3.Data;

namespace LiteTube.DataClasses
{
    public interface IVideoCategory
    {
        string ETag { get; }
        string Id { get; }
        string ChannelId { get; }
        string ChannelETag { get; }
        string Title { get; }
    }

    class MVideoCategory : IVideoCategory
    {
        public MVideoCategory(VideoCategory category)
        {
            ETag = category.ETag;
            Id = category.Id;
            ChannelId = category.Snippet.ChannelId;
            ChannelETag = category.Snippet.ETag;
            Title = category.Snippet.Title;
        }

        public string ETag
        {
            get;
            private set;
        }

        public string Id
        {
            get;
            private set;
        }

        public string ChannelId
        {
            get;
            private set;
        }

        public string ChannelETag
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }
    }
}
