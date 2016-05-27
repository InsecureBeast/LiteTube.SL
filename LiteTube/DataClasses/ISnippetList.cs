using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LiteTube.DataClasses
{
    /// <summary>
    /// Интерфейс для запроса списка сниппетов по странично
    /// </summary>
    public interface ISnippetList : IResponceList
    {
        IList<ISnippet> Items { get; }
        string Kind { get; }
    }

    class MSnippetList : ISnippetList
    {
        public MSnippetList(SearchListResponse searchListResponse, IEnumerable<IVideoDetails> videoDetails)
        {
            Items = new List<ISnippet>();
            foreach (var details in videoDetails)
            {
                var item = searchListResponse.Items.FirstOrDefault(i => i.Id.VideoId == details.VideoId);
                if (item != null)
                    Items.Add(new MSnippet(item, details));
            }
            Kind = searchListResponse.Kind;
            NextPageToken = searchListResponse.NextPageToken;
            PageInfo = new MPageInfo(searchListResponse.PageInfo);
            PrevPageToken = searchListResponse.PrevPageToken;
            VisitorId = searchListResponse.VisitorId;
        }

        public IList<ISnippet> Items
        {
            get;
            private set;
        }

        public string Kind
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
}
