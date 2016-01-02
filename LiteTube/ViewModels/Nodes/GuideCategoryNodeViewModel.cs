using System;
using LiteTube.DataClasses;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.ViewModels
{
    class GuideCategoryNodeViewModel : NodeViewModelBase
    {
        public GuideCategoryNodeViewModel(IGuideCategory category)
        {
            Title = category.Title;
            CategoryId = category.Id;
            ChannelId = category.ChannelId;
            Image = category.Image;
        }

        public string Title
        {
            get; 
            private set;
        }

        public string CategoryId
        {
            get;
            private set;
        }

        public string ChannelId
        {
            get;
            private set;
        }

        public string Image
        {
            get; 
            private set; 
        }

        public override string Id
        {
            get { return null; }
        }

        public override string VideoId
        {
            get { return null; }
        }
    }
}
