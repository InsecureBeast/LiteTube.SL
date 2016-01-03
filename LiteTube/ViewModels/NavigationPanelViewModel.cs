﻿using System.Windows.Input;
using LiteTube.Common;
using LiteTube.DataModel;
using MyToolkit.Command;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels
{
    public class NavigationPanelViewModel : PropertyChangedBase, IListener<UpdateContextEventArgs>
    {
        private readonly IDataSource _datasource;
        private readonly Common.RelayCommand _loginCommand;
        private readonly Common.RelayCommand _logoutCommand;
        private readonly Common.RelayCommand _homeCommand;
        private readonly Common.RelayCommand _settingsCommand;
        private readonly RelayCommand<object> _searchCommand;
        private readonly RelayCommand<string> _channelCommand;
        private bool _isMenuSelected = false;
        private bool _isSettingsSelected = false;
        private bool _isSubscribtionSelected = false;
        private string _profileImage;
        private string _profileDisplayName;
        private bool _isProfileChecked;
        private string _profileRegistered;
        private string _profileSecondDisplayName;

        public NavigationPanelViewModel(IDataSource datasource)
        {
            _datasource = datasource;
            _loginCommand = new Common.RelayCommand(Login);
            _logoutCommand = new Common.RelayCommand(Logout);
            _homeCommand = new Common.RelayCommand(Home);
            _settingsCommand = new Common.RelayCommand(Settings, CanSettings);
            _searchCommand = new RelayCommand<object>(Search);
            _channelCommand = new RelayCommand<string>(LoadChannel);

            _datasource.Subscribe(this);
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

        public IDataSource DataSource
        {
            get { return _datasource; }
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
            get { return _datasource.IsAuthorized; }
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

        private void Login()
        {
            _datasource.Login();
            Home();
        }

        private void Logout()
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                await _datasource.Logout();
                Home();
            });
        }

        private void Home()
        {
            NavigationHelper.GoHome();
        }

        private void Settings()
        {
            NavigationHelper.Navigate("/SettingsPage.xaml", new SettingsViewModel(_datasource));
        }

        private bool CanSettings()
        {
            return !_isSettingsSelected;
        }

        private void Search(object page)
        {
            NavigationHelper.Navigate("/SearchPage.xaml", new SearchPageViewModel(_datasource));
        }

        private void LoadChannel(string channelId)
        {
            NavigationHelper.Navigate("/ChannelPage.xaml", new ChannelPageViewModel(channelId, _datasource));
        }

        private void LoadProfileInfo()
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                ProfileImage = null;
                ProfileDisplayName = string.Empty;
                ProfileSecondDisplayName = string.Empty;

                var profile = await _datasource.GetProfile();
                ProfileImage = profile.Image;
                var names = profile.DisplayName.Split(' ');
                if (names.Length >= 1)
                    ProfileDisplayName = names[0];
                if (names.Length >= 2)
                    ProfileSecondDisplayName = names[1];
                if (profile.Registered != null)
                    ProfileRegistered = profile.Registered.Value.ToString("d");
            });
        }

        public void Notify(UpdateContextEventArgs e)
        {
            NotifyOfPropertyChanged(() => IsAuthorized);
        }
    }
}
