using LiteTube.Common;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using Microsoft.Phone.Shell;
using MyToolkit.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace LiteTube.ViewModels.Nodes
{
    public class CommentNodeViewModel 
    {
        private readonly RelayCommand<string> _channelCommand;
        private readonly IDataSource _datasource;
        private readonly IConnectionListener _connectionListener;

        public CommentNodeViewModel(IComment comment, IDataSource datasource, IConnectionListener connectionListener)
        {
            TextDisplay = comment.TextDisplay;
            AuthorDisplayName = comment.AuthorDisplayName;
            AuthorProfileImageUrl = comment.AuthorProfileImageUrl;
            AuthorChannelId = comment.AuthorChannelId;
            LikeCount = comment.LikeCount;
            PublishedAt = comment.PublishedAt;
            PublishedAtRaw = comment.PublishedAtRaw;
            ReplayComments = comment.ReplayComments.Select(c => new CommentNodeViewModel(c, datasource, _connectionListener));
            IsReplay = comment.IsReplay;

            _datasource = datasource;
            _connectionListener = connectionListener;
            _channelCommand = new RelayCommand<string>(LoadChannel);
        }

        public string TextDisplay { get; private set; }
        public string AuthorDisplayName { get; private set; }
        public string AuthorProfileImageUrl { get; private set; }
        public string AuthorChannelId { get; private set; }
        public long? LikeCount { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public string PublishedAtRaw { get; private set; }
        public IEnumerable<CommentNodeViewModel> ReplayComments { get; private set; }
        public bool IsReplay { get; private set; }

        public ICommand ChannelCommand
        {
            get { return _channelCommand; }
        }

        private void LoadChannel(string channelId)
        {
            PhoneApplicationService.Current.State["model"] = new ChannelPageViewModel(channelId, _datasource, _connectionListener);
            App.NavigateTo("/ChannelPage.xaml");
        }
    }
}
