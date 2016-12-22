using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.Common;

namespace LiteTube.ViewModels.Nodes
{
    public class VideoCategoryNodeViewModel : NodeViewModelBase
    {
        private readonly string _title;
        
        public VideoCategoryNodeViewModel(IVideoCategory category, IDataSource dataSource, IContextMenuProvider menuProvider = null) : base(dataSource, menuProvider)
        {
            _title = category.Title;
            CategoryId = category.Id;
            ChannelId = category.ChannelId;
        }

        public string Title
        {
            get { return _title; }
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
            get { return null; }
        }

        public override string Id
        {
            get { return CategoryId; }
        }

        public override string VideoId
        {
            get { return CategoryId; }
        }
    }
}
