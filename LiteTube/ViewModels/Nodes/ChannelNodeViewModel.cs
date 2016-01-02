using System;
using LiteTube.DataClasses;

namespace LiteTube.ViewModels.Nodes
{
    public class ChannelNodeViewModel : NodeViewModelBase
    {
        private readonly IChannel _channel;
        private string _id;

        public ChannelNodeViewModel(IChannel channel)
        {
            _channel = channel;
            Title = channel.Title;
            _id = channel.Id;
            Image = channel.Thumbnails.Medium.Url;
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
