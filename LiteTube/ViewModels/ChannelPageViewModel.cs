using System;
using System.Windows.Input;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;
using LiteTube.Common;
using LiteTube.Common.Helpers;
using System.Diagnostics;
using LiteTube.ViewModels.Playlist;

namespace LiteTube.ViewModels
{
    class ChannelPageViewModel : SectionBaseViewModel, IPlaylistsSevice
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
        private PlaylistListViewModel _playlistListViewModel;
        private int _selectedIndex;
        private ulong? _channelViewCount;

        public ChannelPageViewModel(string channelId, string username, Func<IDataSource> getDataSource, IConnectionListener connectionListener)
            : base(getDataSource, connectionListener, null)
        {
            _channelId = channelId;
            ShowAdv = SettingsHelper.IsAdvVisible;
            InitializeCommands();
            _playlistListViewModel = new PlaylistListViewModel(channelId, getDataSource, connectionListener, new NoContextMenuStrategy());
            _playlistListViewModel.ShowAdv = SettingsHelper.IsAdvVisible;

            _playlistService = this;

            LayoutHelper.InvokeFromUiThread(async() => 
            {
                await LoadChannel(channelId, username);
                await FirstLoad();
            });
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnSelectedIndexChanged(_selectedIndex);
                NotifyOfPropertyChanged(() => SelectedIndex);
            }
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

        public ulong? ChannelViewCount
        {
            get { return _channelViewCount; }
            private set
            {
                _channelViewCount = value;
                NotifyOfPropertyChanged(() => ChannelViewCount);
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

        public PlaylistListViewModel PlaylistListViewModel
        {
            get { return _playlistListViewModel; }
        }

        public PlaylistsContainerViewModel PlaylistContainerListViewModel
        {
            get { return App.ViewModel.PlaylistListViewModel; }
        }

        public void ShowContainer(bool show, string videoId)
        {
            PlaylistContainerListViewModel.IsContainerShown = true;
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                PlaylistContainerListViewModel.SetVideoId(videoId);
                await PlaylistContainerListViewModel.FirstLoad();
            });
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getDataSource().GetChannelVideoList(_channelId, nextPageToken);
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
            _subscribeCommand = new SubscribeCommand(_getDataSource, () => _channelId, InvalidateCommands);
            _unsubscribeCommand = new UnsubscribeCommand(_getDataSource, () => _channelId, InvalidateCommands);
        }

        private async Task LoadChannel(string channelId, string username)
        {
            IChannel ch;
            if (string.IsNullOrEmpty(username))
                ch = await _getDataSource().GetChannel(channelId);
            else
                ch = await _getDataSource().GetChannelByUsername(username);

            _uniqueId = ch.Id;
            Title = ch.Title;
            Description = ch.Description;
            ChannelSubscribers = ch.Statistics.SubscriberCount;
            ChannelVideoCount = ch.Statistics.VideoCount;
            ChannelViewCount = ch.Statistics.ViewCount;
            ChannelImage = ch.Thumbnails.GetThumbnailUrl();
            Image = ch.Image;
            _channel = ch;
            IsSubscribed = _getDataSource().IsSubscribed(channelId);
        }

        private void InvalidateCommands()
        {
            IsSubscribed = _getDataSource().IsSubscribed(_channelId);
        }

        private async void OnSelectedIndexChanged(int index)
        {
            switch (index)
            {
                case 0:
                    break;

                case 1:
                    Debug.WriteLine("playlist list");
                    if (PlaylistListViewModel.Items.Count > 0)
                        return;

                    await PlaylistListViewModel.FirstLoad();
                    break;

                case 2:
                    break;
            }
        }
    }
}
