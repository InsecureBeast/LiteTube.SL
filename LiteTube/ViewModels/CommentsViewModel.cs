using System;
using System.Windows.Input;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.ViewModels.Nodes;
using System.Net;

namespace LiteTube.ViewModels
{
    public class CommentsViewModel : SectionBaseViewModel
    {
        private readonly string _videoId;
        private IProfile _profile;
        private readonly RelayCommand _addCommentCommand;
        private string _commentText;
        private bool _isAddingComment;

        public CommentsViewModel(string videoId, Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            if (string.IsNullOrEmpty(videoId))
                return;

            _videoId = videoId;
            _addCommentCommand = new RelayCommand(AddComment);
            LoadProfile();
        }

        public bool IsAuthorized
        {
            get { return _getDataSource().IsAuthorized; }
        }

        public string ProfileImage
        {
            get
            {
                if (_profile != null)
                    return _profile.Image;

                return string.Empty;
            }
        }

        public ICommand AddCommentCommand
        {
            get { return _addCommentCommand; }
        }

        public string CommentText
        {
            get { return _commentText; }
            set
            {
                _commentText = value;
                NotifyOfPropertyChanged(() => CommentText);
            }
        }

        public bool IsAddingComment
        {
            get { return _isAddingComment; }
            set
            {
                _isAddingComment = value;
                NotifyOfPropertyChanged(() => IsAddingComment);
            }
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            if (Items.Count > 0)
                IsLoading = false;

            return await _getDataSource().GetComments(_videoId, nextPageToken);
        }

        internal override void LoadItems(IResponceList videoList)
        {
            var list = videoList as ICommentList;
            if (list == null)
                return;

            AddItems(list.Items);
        }

        private void AddItems(IEnumerable<IComment> items)
        {
            foreach (var item in items)
            {
                Items.Add(new CommentNodeViewModel(item, _getDataSource, _connectionListener));
                foreach (var replayItem in item.ReplayComments.OrderBy(c => c.PublishedAt))
                {
                    Items.Add(new CommentNodeViewModel(replayItem, _getDataSource, _connectionListener));
                }
            }
        }

        private void LoadProfile()
        {
            _profile = _getDataSource().GetProfile();
            NotifyOfPropertyChanged(() => ProfileImage);
        }
        
        private async void AddComment()
        {
            try
            {
                IsAddingComment = true;
                var myChannelId = _profile.ChannelId;
                var myComment = await _getDataSource().AddComment(myChannelId, _videoId, CommentText);
                if (myComment == null)
                {
                    IsAddingComment = false;
                    CommentText = string.Empty;
                    return;
                }
                Items.Insert(0, new CommentNodeViewModel(myComment, _getDataSource, _connectionListener));
                CommentText = string.Empty;
                IsAddingComment = false;
                IsEmpty = false;
            }
            catch (WebException e)
            {
                IsAddingComment = false;
                CommentText = string.Empty;
                throw new LiteTubeException(e);
            }      
        }
    }
}
