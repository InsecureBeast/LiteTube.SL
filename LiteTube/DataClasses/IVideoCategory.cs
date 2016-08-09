using Google.Apis.YouTube.v3.Data;

namespace LiteTube.DataClasses
{
    public interface IVideoCategory
    {
        string Id { get; }
        string ChannelId { get; }
        string Title { get; }
    }

    class MVideoCategory : IVideoCategory
    {
        public MVideoCategory(VideoCategory category)
        {
            Id = category.Id;
            if (category.Snippet == null)
                return;

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
