using System;

namespace LiteTube.DataClasses
{
    public interface IProfile
    {
        string Image { get; }
        string DisplayName { get; }
        DateTime? Registered { get; }
        string ChannelId { get; }
    }

    class MProfile : IProfile
    {
        public MProfile(string channelId, string image, string displayName)
        {
            Image = image;
            DisplayName = displayName;
            ChannelId = channelId;
        }

        public MProfile(string channelId, string image, string displayName, DateTime? registered)
            : this(channelId, image, displayName)
        {
            Registered = registered;
        }

        public string DisplayName
        {
            get;
            private set;
        }

        public string Image
        {
            get;
            private set;
        }

        public DateTime? Registered
        {
            get;
            private set;
        }

        public string ChannelId
        {
            get; private set;
        }
    }
}
