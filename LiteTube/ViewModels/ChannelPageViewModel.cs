using System.Windows.Input;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;
using LiteTube.Common;

namespace LiteTube.ViewModels
{
    class ChannelPageViewModel : SectionBaseViewModel
    {
        private IChannel _channel;
        private readonly string _channelId;
        private ulong? _channelVideoCount;
        private ulong? _channelSubscribers;
        private string _image;
        private string _channelImage;
        private SubscribeCommand _subscribeCommand;
        private UnsubscribeCommand _unsubscribeCommand;
        private bool _isSubscribed;

        public ChannelPageViewModel(string channelId, IDataSource dataSource, IConnectionListener connectionListener)
            : base(dataSource, connectionListener)
        {
            _channelId = channelId;

            InitializeCommands();
            LoadChannel(channelId);
            LayoutHelper.InvokeFromUIThread(async() => await FirstLoad());
        }

        public override string ToString()
        {
            return Title;
        }

        public string Image
        {
            get { return _image; }
            private set
            {
                _image = value;
                NotifyOfPropertyChanged(() => Image);
            }
        }

        public string ChannelImage
        {
            get { return _channelImage; }
            private set
            {
                _channelImage = value;
                NotifyOfPropertyChanged(() => ChannelImage);
            }
        }

        public ulong? ChannelVideoCount
        {
            get { return _channelVideoCount; }
            private set
            {
                _channelVideoCount = value;
                NotifyOfPropertyChanged(() => ChannelVideoCount);
            }
        }

        public ulong? ChannelSubscribers
        {
            get { return _channelSubscribers; }
            private set
            {
                _channelSubscribers = value;
                NotifyOfPropertyChanged(() => ChannelSubscribers);
            }
        }

        public ICommand SubscribeCommand
        {
            get { return _subscribeCommand; }
        }

        public ICommand UnsubscribeCommand
        {
            get { return _unsubscribeCommand; }
        }

        public bool IsSubscribed
        {
            get { return _isSubscribed; }
            set
            {
                _isSubscribed = value;
                NotifyOfPropertyChanged(() => IsSubscribed);
            }
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _dataSource.GetChannelVideoList(_channelId, nextPageToken);
        }

        internal override void LoadItems(IResponceList videoList)
        {
            var snippetList = videoList as IVideoList;
            if (snippetList == null)
                return;

            foreach (var item in snippetList.Items)
            {
                item.ChannelTitle = _channel.Title;
            }

            AddItems(snippetList.Items);
        }

        private void InitializeCommands()
        {
            _subscribeCommand = new SubscribeCommand(_dataSource, () => _channelId, InvalidateCommands);
            _unsubscribeCommand = new UnsubscribeCommand(_dataSource, () => _channelId, InvalidateCommands);
        }

        private void LoadChannel(string channelId)
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                var ch = await _dataSource.GetChannel(channelId);
                _uniqueId = ch.Id;
                Title = ch.Title;
                Description = ch.Description;
                ChannelSubscribers = ch.Statistics.SubscriberCount;
                ChannelVideoCount = ch.Statistics.VideoCount;
                ChannelImage = ch.Thumbnails.Medium.Url;
                Image = ch.Image;
                _channel = ch;
                IsSubscribed = _dataSource.IsSubscribed(channelId);
            });
        }

        private void InvalidateCommands()
        {
            IsSubscribed = _dataSource.IsSubscribed(_channelId);
        }
    }
}
