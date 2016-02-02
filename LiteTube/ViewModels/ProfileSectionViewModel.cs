using LiteTube.DataModel;
using MyToolkit.Command;
using System.Windows.Input;
using System;
using System.Windows;
using Microsoft.Phone.Shell;
using LiteTube.Common;

namespace LiteTube.ViewModels
{
    public class ProfileSectionViewModel : PropertyChangedBase, IListener<UpdateContextEventArgs>, IListener<ConnectionEventArgs>
    {
        private readonly RelayCommand<FrameworkElement> _subscriptionsCommand;
        private readonly RelayCommand<FrameworkElement> _historyCommand;
        private readonly RelayCommand<FrameworkElement> _recommendedCommand;
        private readonly RelayCommand<FrameworkElement> _videoCategoryCommand;
        private readonly RelayCommand<FrameworkElement> _favoritesCommand;
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

        public ProfileSectionViewModel(Func<IDataSource> getDatasource, IConnectionListener connectionListener)
        {
            _getDataSource = getDatasource;
            _connectionListener = connectionListener;
            _subscriptionsCommand = new RelayCommand<FrameworkElement>(Subscriptions);
            _historyCommand = new RelayCommand<FrameworkElement>(GetHistory);
            _recommendedCommand = new RelayCommand<FrameworkElement>(Recommended);
            _videoCategoryCommand = new RelayCommand<FrameworkElement>(VideoCategories);
            _favoritesCommand = new RelayCommand<FrameworkElement>(Favorites);
            _loginCommand = new Common.RelayCommand(Login);
            _logoutCommand = new Common.RelayCommand(Logout);
            _getDataSource().Subscribe(this);
            _connectionListener.Subscribe(this);
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

        private void Recommended(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(0, _getDataSource, _connectionListener);
            App.NavigateTo("/MenuPage.xaml?item=0");
        }

        private void Subscriptions(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(1, _getDataSource, _connectionListener);
            App.NavigateTo("/MenuPage.xaml?item=1");
        }

        private void GetHistory(FrameworkElement control)
        {
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(3, _getDataSource, _connectionListener);
            App.NavigateTo("/MenuPage.xaml?item=3");
        }

        private void VideoCategories(FrameworkElement control)
        {
            var index = IsAuthorized ? 4 : 0;
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(index, _getDataSource, _connectionListener);
            App.NavigateTo(string.Format("/MenuPage.xaml?item={0}", index));
        }

        private void Favorites(FrameworkElement control)
        {
            var index = 2;
            PhoneApplicationService.Current.State["model"] = new MenuPageViewModel(index, _getDataSource, _connectionListener);
            App.NavigateTo(string.Format("/MenuPage.xaml?item={0}", index));
            ////var index = IsAuthorized ? 2 : 0; //TODO favorites saved on device!!
        }

        public void Notify(UpdateContextEventArgs e)
        {
            NotifyOfPropertyChanged(() => IsAuthorized);
            LoadProfileInfo();
        }

        private async void Login()
        {
            await _getDataSource().Login();
        }

        private void Logout()
        {
            _getDataSource().Logout();
        }

        private void LoadProfileInfo()
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                ProfileImage = null;
                ProfileDisplayName = string.Empty;
                ProfileSecondDisplayName = string.Empty;
                ProfileRegistered = null;

                var profile = await _getDataSource().GetProfile();
                ProfileImage = profile.Image;
                var names = profile.DisplayName.Split(' ');
                if (names.Length >= 1)
                    ProfileDisplayName = names[0];
                if (names.Length >= 2)
                    ProfileSecondDisplayName = names[1];
                if (profile.Registered != null)
                    ProfileRegistered = profile.Registered.Value.ToString("d");

                ProfileChannelId = profile.ChannelId;
            });
        }

        public void Notify(ConnectionEventArgs e)
        {
            if (e.IsConnected)
                LoadProfileInfo();
        }
    }
}
