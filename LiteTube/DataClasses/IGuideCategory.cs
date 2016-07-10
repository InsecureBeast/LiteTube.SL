using Google.Apis.YouTube.v3.Data;

namespace LiteTube.DataClasses
{
    public interface IGuideCategory
    {
        string Id { get; }
        string ChannelId { get; }
        string Title { get; }
    }

    class MGuideCategory : IGuideCategory
    {
        public MGuideCategory(GuideCategory category)
        {
            Id = category.Id;
            ChannelId = category.Snippet.ChannelId;
            Title = category.Snippet.Title;
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

        public string Title
        {
            get;
            private set;
        }
    }
}
