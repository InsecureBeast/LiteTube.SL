﻿using System;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.ViewModels.Nodes;
using LiteTube.Common;
using LiteTube.Common.Helpers;
using LiteTube.ViewModels.Playlist;
#if SILVERLIGHT
using Microsoft.Phone.Shell;
using LiteTube.Resources;
#endif

namespace LiteTube.ViewModels
{
    public class MainViewModel : SectionBaseViewModel, IListener<UpdateContextEventArgs>, IListener<UpdateSettingsEventArgs>, IPlaylistsSevice
    {
        private readonly MostPopularViewModel _mostPopularViewModel;
        private readonly ProfileSectionViewModel _profileSectionViewModel;
        private readonly ActivitySectionViewModel _activitySectionViewModel;
        private readonly PlaylistsContainerViewModel _playlistViewModel;
#if SILVERLIGHT
        private readonly ProgressIndicatorHolder _indicatorHolder;
#endif

        public MainViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener, null)
        {
#if SILVERLIGHT
            _indicatorHolder = new ProgressIndicatorHolder();
#endif
            _mostPopularViewModel = new MostPopularViewModel(_getDataSource, _connectionListener, this);
            _profileSectionViewModel = new ProfileSectionViewModel(_getDataSource, connectionListener);
            _activitySectionViewModel = new ActivitySectionViewModel(_getDataSource, _connectionListener, this);
            _playlistViewModel = new PlaylistsContainerViewModel(_getDataSource, _connectionListener);

            geDataSource().Subscribe((IListener<UpdateSettingsEventArgs>)this);
            geDataSource().Subscribe((IListener<UpdateContextEventArgs>)this);

            _hasItems = false;
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public MostPopularViewModel MostPopularViewModel
        {
            get { return _mostPopularViewModel; }
        }

        public ProfileSectionViewModel ProfileSectionViewModel
        {
            get { return _profileSectionViewModel; }
        }

        public ActivitySectionViewModel ActivitySectionViewModel
        {
            get { return _activitySectionViewModel; }
        }

        public bool IsAuthorized
        {
            get
            { 
                //return _getGeDataSource().IsAuthorized; 
                return SettingsHelper.IsContainsAuthorizationData();
            }
        }

        public IConnectionListener ConnectionListener
        {
            get { return _connectionListener; }
        }

#if SILVERLIGHT
        public ProgressIndicatorHolder IndicatorHolder
        {
            get { return _indicatorHolder; }
        }
#endif

        public PlaylistsContainerViewModel PlaylistListViewModel
        {
            get { return _playlistViewModel; }
        }

        /// <summary>
        /// Creates and adds a few VideoItemViewModel objects into the Items collection.
        /// </summary>
        public async Task LoadData()
        {
            if (IsDataLoaded)
                return;
            
            IsLoading = true;
            IsEmpty = false;

            LoadActivities();
            LoadMostPopular();
            await LoadGuideCategories();

            IsDataLoaded = true;
            IsLoading = false;
            IsEmpty = !Items.Any();
        }

        public async Task ReloadData()
        {
            IsDataLoaded = false;
            await LoadData();
        }

        private async Task LoadGuideCategories()
        {
            var sections = await _getDataSource().GetCategories();
            if (sections == null)
                return;

            Items.Clear();
            foreach (var section in sections)
            {
                Items.Add(new VideoCategoryNodeViewModel(section, _getDataSource()));
            }
        }

        private async void LoadActivities()
        {
            if (!_getDataSource().IsAuthorized)
                return;

            _activitySectionViewModel.Items.Clear();
            await _activitySectionViewModel.FirstLoad();
        }

        private async void LoadMostPopular()
        {
            _mostPopularViewModel.Items.Clear();
            await _mostPopularViewModel.FirstLoad();
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            if (navObject == null)
                return;

            var viewModel = navObject.ViewModel as VideoCategoryNodeViewModel;
            if (viewModel == null)
                return;

            var categoryId = viewModel.CategoryId;
            var title = viewModel.Title;
#if SILVERLIGHT
            PhoneApplicationService.Current.State["model"] = new VideoCategorySectionViewModel(categoryId, title, _getDataSource, _connectionListener);
            App.NavigateTo("/SectionPage.xaml");
#endif
        }

        public async void Notify(UpdateContextEventArgs e)
        {
            await ReloadData();
            NotifyOfPropertyChanged(() => IsAuthorized);
        }

        public async void Notify(UpdateSettingsEventArgs e)
        {
            await ReloadData();
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                NotifyOfPropertyChanged(() => IsAuthorized);
                NotifyOfPropertyChanged(() => IsConnected);

                if (!IsConnected)
                    return;

                IsLoading = true;
                IsEmpty = false;

                await LoadGuideCategories();
                if (IsAuthorized)
                {
                    _activitySectionViewModel.Items.Clear();
                    await _activitySectionViewModel.FirstLoad();
                }
                IsDataLoaded = true;
                IsLoading = false;
                IsEmpty = !Items.Any();
            });
        }

        public void ShowContainer(bool show, string videoId)
        {
            _playlistViewModel.IsContainerShown = show;
            LayoutHelper.InvokeFromUiThread(async () =>
            {
                _playlistViewModel.SetVideoId(videoId);
                await _playlistViewModel.FirstLoad();
            });
        }
    }
}