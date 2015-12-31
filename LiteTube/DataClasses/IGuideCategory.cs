using Google.Apis.YouTube.v3.Data;

namespace LiteTube.DataClasses
{
    public interface IGuideCategory
    {
        string ETag { get; }
        string Id { get; }
        string ChannelId { get; }
        string ChannelETag { get; }
        string Title { get; }
        string Image { get; }
    }

    class MGuideCategory : IGuideCategory
    {
        public MGuideCategory(GuideCategory category)
        {
            ETag = category.ETag;
            Id = category.Id;
            ChannelId = category.Snippet.ChannelId;
            ChannelETag = category.Snippet.ETag;
            Title = category.Snippet.Title;
            Image = string.Empty;
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

        public string Image
        {
            get; 
            private set;
        }
    }
}
