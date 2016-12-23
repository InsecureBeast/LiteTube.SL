using MyToolkit.Command;
using System.Windows.Input;
using System;
using LiteTube.Common;
using LiteTube.DataModel;
using System.Windows;
using LiteTube.Common.Helpers;
using LiteTube.Resources;
using LiteTube.ViewModels.Playlist;
#if SILVERLIGHT
using Microsoft.Phone.Shell;
#else
using Windows.UI.Xaml;
#endif

namespace LiteTube.ViewModels
{
    public class ProfileSectionViewModel : PropertyChangedBase, IListener<UpdateContextEventArgs>, IListener<ConnectionEventArgs>
    {
        private readonly RelayCommand<FrameworkElement> _subscriptionsCommand;
        private readonly RelayCommand<FrameworkElement> _historyCommand;
        private readonly RelayCommand<FrameworkElement> _recommendedCommand;
        private readonly RelayCommand<FrameworkElement> _videoCategoryCommand;
        private readonly RelayCommand<FrameworkElement> _favoritesCommand;
        private readonly RelayCommand<FrameworkElement> _likedCommand;
        private readonly RelayCommand<FrameworkElement> _uploadedCommand;
        private readonly RelayCommand<FrameworkElement> _myPlaylistsCommand;
        private readonly RelayCommand<FrameworkElement> _watchLaterCommand;
        private readonly Common.RelayCommand _loginCommand;
        private readonly Common.RelayCommand _logoutCommand;
        private readonly Func<IDataSource> _getDataSource;
        private readonly IConnectionListener _connectionListener;

        private string _profileImage;
        private string _profileDisplayName;
        private bool _isProfileChecked;
        private string _profileRegistered;
        private string _profileSecondDisplayName;
        private string _profileChannelId;
        private LoginStatus _loginStatus;
        private bool _loginProcess;

        public ProfileSectionViewModel(Func<IDataSource> getDatasource, IConnectionListener connectionListener)
        {
            _getDataSource = getDatasource;
            _connectionListener = connectionListener;
            _subscriptionsCommand = new RelayCommand<FrameworkElement>(Subscriptions);
            _historyCommand = new RelayCommand<FrameworkElement>(GetHistory);
            _recommendedCommand = new RelayCommand<FrameworkElement>(Recommended);
            _videoCategoryCommand = new RelayCommand<FrameworkElement>(VideoCategories);
            _favoritesCommand = new RelayCommand<FrameworkElement>(Favorites);
            _likedCommand = new RelayCommand<FrameworkElement>(Liked);
            _uploadedCommand = new RelayCommand<FrameworkElement>(Uploaded);
            _myPlaylistsCommand = new RelayCommand<FrameworkElement>(MyPlaylists);
            _watchLaterCommand = new RelayCommand<FrameworkElement>(WatchLater);
            _loginCommand = new Common.RelayCommand(Login);
            _logoutCommand = new Common.RelayCommand(Logout);
            _getDataSource().Subscribe(this);
            _connectionListener.Subscribe(this);
            _loginStatus = LoginStatus.NotLogged;
        }

        public ICommand SubsribtionsCommand
        {
            get { return _subscriptionsCommand; }
        }

        public ICommand HistoryCommand
        {
            get { return _historyCommand; }
        }

        public ICommand RecommendedCommand
        {
            get { return _recommendedCommand; }
        }

        public ICommand VideoCategoryCommand
        {
            get { return _videoCategoryCommand; }
        }

        public ICommand FavoritesCommand
        {
            get { return _favoritesCommand; }
        }

        public ICommand LikedCommand
        {
            get { return _likedCommand; }
        }

        public ICommand UploadedCommand
        {
            get { return _uploadedCommand; }
        }

        public ICommand MyPlaylistsCommand
        {
            get { return _myPlaylistsCommand; }
        }

        public ICommand WatchLaterCommand
        {
            get { return _watchLaterCommand; }
        }

        public ICommand LoginCommand
        {
            get { return _loginCommand; }
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand; }
        }

        public bool IsAuthorized
        {
            get { return _getDataSource().IsAuthorized; }
        }

        public LoginStatus LoginStatus
        {
            get { return _loginStatus; }
            set
            {
                _loginStatus = value;
                NotifyOfPropertyChanged(() => LoginStatus);
            }
        }

        public string ProfileImage
        {
            get { return _profileImage; }
            set
            {
                _profileImage = value;
                NotifyOfPropertyChanged(() => ProfileImage);
            }
        }

        public string ProfileDisplayName
        {
            get { return _profileDisplayName; }
            set
            {
                _profileDisplayName = value;
                NotifyOfPropertyChanged(() => ProfileDisplayName);
            }
        }

        public string ProfileSecondDisplayName
        {
            get { return _profileSecondDisplayName; }
            set
            {
                _profileSecondDisplayName = value;
                NotifyOfPropertyChanged(() => ProfileSecondDisplayName);
            }
        }

        public string ProfileRegistered
        {
            get { return _profileRegistered; }
            set
            {
                _profileRegistered = value;
                NotifyOfPropertyChanged(() => ProfileRegistered);
            }
        }

        public bool IsProfileChecked
        {
            get { return _isProfileChecked; }
            set
            {
                _isProfileChecked = value;
                if (_isProfileChecked)
                {
                    LoadProfileInfo();
                }
            }
        }

        public string ProfileChannelId
        {
            get { return _profileChannelId; }
            set
            {
                NotifyOfPropertyChanged(() => ProfileChannelId);
                _profileChannelId = value;
            }
        }

        public void Notify(UpdateContextEventArgs e)
        {
            LoadProfileInfo();
        }

        public void Notify(ConnectionEventArgs e)
        {
            if (e.IsConnected)
                LoadProfileInfo();
        }

        private void VideoCategories(FrameworkElement control)
        {
            //var index = IsAuthorized ? 7 : 0;
            NavigateTo(0);
        }

        private void Recommended(FrameworkElement control)
        {
            NavigateTo(1);
        }

        private void Subscriptions(FrameworkElement control)
        {
            NavigateTo(2);
        }

        private void MyPlaylists(FrameworkElement control)
        {
            NavigateTo(3);
        }

        private void WatchLater(FrameworkElement control)
        {
#if SILVERLIGHT
            var id = _getDataSource().WatchLaterPlaylistId;
            var view = string.Format("/PlaylistPage.xaml", id);
            NavigationHelper.Navigate(view, new PlaylistPageViewModel(id, AppResources.WatchLater, _getDataSource, _connectionListener));
#endif
        }

        private void Favorites(FrameworkElement control)
        {
            NavigateTo(4);
        }

        private void Liked(FrameworkElement control)
        {
            NavigateTo(5);
        }

        private void Uploaded(FrameworkElement control)
        {
            NavigateTo(6);
        }

        private void GetHistory(FrameworkElement control)
        {
            NavigateTo(7);
        }

        private void NavigateTo(int index)
        {
#if SILVERLIGHT
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(index, _getDataSource, _connectionListener);
            App.NavigateTo(string.Format("/MenuPage.xaml?item={0}", index));
#endif
        }

        private async void Login()
        {
            _loginProcess = true;
            await _getDataSource().Login();
        }

        private void Logout()
        {
            _getDataSource().Logout();
        }

        private void LoadProfileInfo()
        {
            ProfileImage = null;
            ProfileDisplayName = string.Empty;
            ProfileSecondDisplayName = string.Empty;
            ProfileRegistered = null;

            var profile = _getDataSource().GetProfile();
            if (profile == null)
            {
                if (_loginProcess)
                    LoginStatus = LoginStatus.NotFound;
                else
                    LoginStatus = LoginStatus.NotLogged;
                NotifyOfPropertyChanged(() => IsAuthorized);
                _loginProcess = false;
                return;
            }

            ProfileImage = profile.Image;
            var names = profile.DisplayName.Split(' ');
            if (names.Length >= 1)
                ProfileDisplayName = names[0];
            if (names.Length >= 2)
                ProfileSecondDisplayName = names[1];
            if (profile.Registered != null)
                ProfileRegistered = profile.Registered.Value.ToString("d");

            ProfileChannelId = profile.ChannelId;
            LoginStatus = LoginStatus.Logged;
            NotifyOfPropertyChanged(() => IsAuthorized);
            _loginProcess = false;
        }
    }

    public enum LoginStatus
    {
        Logged,
        NotLogged,
        NotFound
    }
}
