using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using LiteTube.Common;

namespace LiteTube.ViewModels.Nodes
{
    public class ChannelNodeViewModel : NodeViewModelBase
    {
        private readonly IChannel _channel;
        private readonly string _id;

        public ChannelNodeViewModel(IChannel channel, IDataSource dataSource, IContextMenuProvider menuProvider = null) : base(dataSource, menuProvider) 
        {
            _channel = channel;
            Title = channel.Title;
            _id = channel.Id;
            Image = channel.Thumbnails.GetThumbnailUrl();
        }

        public string Title
        {
            get; 
            private set;
        }

        public override string Id
        {
            get { return _id; }
        }

        public override string VideoId
        {
            get { return null; }
        }

        public string Image
        {
            get;
            private set;
        }

        public IChannel Channel 
        {
            get { return _channel; }
        }
    }
}
