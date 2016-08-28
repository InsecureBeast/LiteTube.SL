using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.DataClasses
{
    public class SearchFilter
    {
        public SearchResource.ListRequest.OrderEnum Order { get; set; }
        public SearchResource.ListRequest.VideoDefinitionEnum VideoDefinition { get; set; }
        public SearchResource.ListRequest.VideoDurationEnum VideoDuration { get; set; }
        public DateTime? PublishedAfter { get; set; }
        public DateTime? PublishedBefore { get; set; }

        public SearchFilter()
        {
            Order = SearchResource.ListRequest.OrderEnum.Relevance;
            VideoDefinition = SearchResource.ListRequest.VideoDefinitionEnum.Any;
            VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Any;
            PublishedAfter = null;
            PublishedBefore = null;
        }
    }
}
