using System;
using System.Windows.Input;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.Common.Notifier;
using LiteTube.Core.DataModel;
using LiteTube.Core.ViewModels.Search;
using MyToolkit.Command;
using RelayCommand = LiteTube.Core.Common.Commands.RelayCommand;

namespace LiteTube.Core.ViewModels
{
    public class NavigationPanelViewModel : PropertyChangedBase, IListener<UpdateContextEventArgs>
    {
        private readonly Func<IDataSource> _getDataSource;
        private readonly IConnectionListener _connectionListener;
        private readonly RelayCommand _loginCommand;
        private readonly RelayCommand _logoutCommand;
        private readonly RelayCommand _homeCommand;
        private readonly RelayCommand _settingsCommand;
        private readonly RelayCommand _searchCommand;
        private readonly RelayCommand<string> _channelCommand;
        private bool _isMenuSelected = false;
        private bool _isSettingsSelected = false;
        private bool _isSubscribtionSelected = false;
        private string _profileImage;
        private string _profileDisplayName;
        private bool _isProfileChecked;
        private string _profileRegistered;
        private string _profileSecondDisplayName;
        private string _profileChannelId;

        public NavigationPanelViewModel(Func<IDataSource> getDataSource, IConnectionListener connectionListener)
        {
            _getDataSource = getDataSource;
            _connectionListener = connectionListener;
            _loginCommand = new RelayCommand(Login);
            _logoutCommand = new RelayCommand(Logout);
            _homeCommand = new RelayCommand(Home);
            _settingsCommand = new RelayCommand(Settings, CanSettings);
            _searchCommand = new RelayCommand(Search);
            _channelCommand = new RelayCommand<string>(LoadChannel);
        }

        public ICommand LoginCommand
        {
            get { return _loginCommand; }
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand; }
        }

        public ICommand HomeCommand
        {
            get { return _homeCommand; }
        }

        public ICommand SettingsCommand
        {
            get { return _settingsCommand; }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public ICommand ChannelCommand
        {
            get { return _channelCommand; }
        }

        public bool IsMenuSelected
        {
            get { return _isMenuSelected; }
            set
            {
                _isMenuSelected = value;
                NotifyOfPropertyChanged(() => IsMenuSelected);
            }
        }

        public bool IsSettingsSelected
        {
            get { return _isSettingsSelected; }
            set
            {
                _isSettingsSelected = value;
                NotifyOfPropertyChanged(() => IsSettingsSelected);
                _settingsCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsSubscribtionSelected
        {
            get { return _isSubscribtionSelected; }
            set
            {
                _isSubscribtionSelected = value;
                NotifyOfPropertyChanged(() => IsSubscribtionSelected);
            }
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
                NotifyOfPropertyChanged(() => IsProfileChecked);
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

        private void Login()
        {
            _getDataSource().Login();
            Home();
        }

        private void Logout()
        {
            _getDataSource().Logout();
            Home();
        }

        private void Home()
        {
#if SILVERLIGHT
            NavigationHelper.GoHome();
#endif
        }

        private void Settings()
        {
#if SILVERLIGHT
            NavigationHelper.Navigate("/SettingsPage.xaml", new SettingsViewModel(_getDataSource, _connectionListener));
#endif
        }

        private bool CanSettings()
        {
            return !_isSettingsSelected;
        }

        private void Search()
        {
#if SILVERLIGHT
            NavigationHelper.Navigate("/SearchPage.xaml", new SearchPageViewModel(_getDataSource, _connectionListener));
#endif
        }

        private void LoadChannel(string channelId)
        {
#if SILVERLIGHT
            NavigationHelper.GoToChannelPage(channelId, null);
#endif
        }

        public void Notify(UpdateContextEventArgs e)
        {
            NotifyOfPropertyChanged(() => IsAuthorized);
        }
    }
}
