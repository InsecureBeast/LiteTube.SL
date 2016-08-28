using Google.Apis.YouTube.v3;
using System;

namespace LiteTube.ViewModels.Search
{
    class SearchFilterItem
    {
        public SearchFilterItem(string title, object value)
        {
            Title = title;
            Value = value;
        }

        public string Title { get; private set; }
        public virtual object Value { get; private set; }

        public override string ToString()
        {
            return Title;
        }
    }

    class OrderSearchFilterItem : SearchFilterItem
    {
        public OrderSearchFilterItem(string title, SearchResource.ListRequest.OrderEnum order) : 
            base (title, order)
        {

        }

        public new SearchResource.ListRequest.OrderEnum Value
        {
            get { return (SearchResource.ListRequest.OrderEnum)base.Value; }
        }
    }

    class UploadSearchFilterItem : SearchFilterItem
    {
        public UploadSearchFilterItem(string title, DateTime? datetime) :
            base(title, datetime)
        {

        }

        public new DateTime? Value
        {
            get { return (DateTime?)base.Value; }
        }
    }

    class VideoDefinitionSearchFilterItem : SearchFilterItem
    {
        public VideoDefinitionSearchFilterItem(string title, SearchResource.ListRequest.VideoDefinitionEnum videoDifinition) :
            base(title, videoDifinition)
        {

        }

        public new SearchResource.ListRequest.VideoDefinitionEnum Value
        {
            get { return (SearchResource.ListRequest.VideoDefinitionEnum)base.Value; }
        }
    }

    class VideoDurationSearchFilterItem : SearchFilterItem
    {
        public VideoDurationSearchFilterItem(string title, SearchResource.ListRequest.VideoDurationEnum videoDuration) :
            base(title, videoDuration)
        {

        }

        public new SearchResource.ListRequest.VideoDurationEnum Value
        {
            get { return (SearchResource.ListRequest.VideoDurationEnum)base.Value; }
        }
    }
}
