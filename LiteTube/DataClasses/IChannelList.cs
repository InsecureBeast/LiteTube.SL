using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;
using System.Linq;

namespace LiteTube.DataClasses
{
    public interface IChannelList : IResponceList
    {
        IList<IChannel> Items { get; }
    }

    class MChannelList : IChannelList
    {
        public MChannelList(ChannelListResponse channelListResponse)
        {
            if (channelListResponse == null)
                return;

            ETag = channelListResponse.ETag;
            EventId = channelListResponse.EventId;
            NextPageToken = channelListResponse.NextPageToken;
            PageInfo = new MPageInfo(channelListResponse.PageInfo);
            PrevPageToken = channelListResponse.PrevPageToken;
            //TokenPagination = new MTokenPagination(channelListResponse.TokenPagination);
            VisitorId = channelListResponse.VisitorId;
            Items = channelListResponse.Items.Select(channel => new MChannel(channel)).Cast<IChannel>().ToList();
        }

        public static IChannelList EmptyList
        {
            get { return new MChannelList(null);}
        }

        public string ETag
        {
            get;
            private set;
        }

        public string EventId
        {
            get;
            private set;
        }

        public IList<IChannel> Items
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

        public ITokenPagination TokenPagination
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
}
