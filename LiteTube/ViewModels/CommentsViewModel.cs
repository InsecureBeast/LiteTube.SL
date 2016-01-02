using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.ViewModels
{
    public class CommentsViewModel : SectionBaseViewModel
    {
        private readonly string _videoId;
        private IProfile _profile;
        private IDataSource _dataSource;
        private ObservableCollection<CommentNodeViewModel> _comments;

        public CommentsViewModel(string videoId, IDataSource dataSource) : base(dataSource)
        {
            _videoId = videoId;
            _dataSource = dataSource;
            LoadProfile();
            _comments = new ObservableCollection<CommentNodeViewModel>();
        }

        public ObservableCollection<CommentNodeViewModel> Comments
        {
            get { return _comments; }
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

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await base._dataSource.GetComments(_videoId, nextPageToken);
        }

        internal override void LoadItems(IResponceList videoList)
        {
            var list = videoList as ICommentList;
            if (list == null)
                return;

            AddItems(list.Items);
        }

        internal void AddItems(IEnumerable<IComment> items)
        {
            foreach (var item in items)
            {
                _comments.Add(new CommentNodeViewModel(item, _dataSource));
                foreach (var replayItem in item.ReplayComments.OrderBy(c => c.PublishedAt))
                {
                    _comments.Add(new CommentNodeViewModel(replayItem, _dataSource));
                }
            }

            IsLoading = false;
            if (!_comments.Any())
                IsEmpty = true;
        }

        private void LoadProfile()
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                _profile = await _dataSource.GetProfile();
                NotifyOfPropertyChanged(() => ProfileImage);
            });
        }
    }
}
