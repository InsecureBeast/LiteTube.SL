using System;
using LiteTube.DataClasses;
using LiteTube.ViewModels.Nodes;
using LiteTube.DataModel;
using LiteTube.Common;

namespace LiteTube.ViewModels
{
    class GuideCategoryNodeViewModel : NodeViewModelBase
    {
        public GuideCategoryNodeViewModel(IGuideCategory category, IDataSource dataSource, IContextMenuStrategy menuProvider = null) : base(dataSource, menuProvider, false)
        {
            Title = category.Title;
            CategoryId = category.Id;
            ChannelId = category.ChannelId;
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
