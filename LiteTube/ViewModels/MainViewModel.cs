using System;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.ViewModels.Nodes;
using LiteTube.Common;
using LiteTube.Common.Helpers;
#if SILVERLIGHT
using Microsoft.Phone.Shell;
using LiteTube.Resources;
#endif

namespace LiteTube.ViewModels
{
    public class MainViewModel : SectionBaseViewModel, IListener<UpdateContextEventArgs>, IListener<UpdateSettingsEventArgs>
    {
        private readonly MostPopularViewModel _mostPopularViewModel;
        private readonly ProfileSectionViewModel _profileSectionViewModel;
        private readonly ActivitySectionViewModel _activitySectionViewModel;
#if SILVERLIGHT
        private readonly ProgressIndicatorHolder _indicatorHolder;
#endif

        public MainViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
#if SILVERLIGHT
            _indicatorHolder = new ProgressIndicatorHolder();
#endif
            _mostPopularViewModel = new MostPopularViewModel(_getGeDataSource, _connectionListener);
            _profileSectionViewModel = new ProfileSectionViewModel(_getGeDataSource, connectionListener);
            _activitySectionViewModel = new ActivitySectionViewModel(_getGeDataSource, _connectionListener);

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

        /// <summary>
        /// Creates and adds a few VideoItemViewModel objects into the Items collection.
        /// </summary>
        public async Task LoadData()
        {
            if (IsDataLoaded)
                return;
            
            IsLoading = true;
            IsEmpty = false;

            _mostPopularViewModel.Items.Clear();
            if (_getGeDataSource().IsAuthorized)
            {
                _activitySectionViewModel.Items.Clear();
                _activitySectionViewModel.FirstLoad();
            }

            _mostPopularViewModel.FirstLoad();
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
            var sections = await _getGeDataSource().GetCategories();
            if (sections == null)
                return;

            Items.Clear();
            foreach (var section in sections)
            {
                Items.Add(new VideoCategoryNodeViewModel(section));
            }
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
            PhoneApplicationService.Current.State["model"] = new VideoCategorySectionViewModel(categoryId, title, _getGeDataSource, _connectionListener);
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
                IsConnected = e.IsConnected;
                NotifyOfPropertyChanged(() => IsAuthorized);

                if (e.IsConnected)
                {
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
                    return;
                }

                if (Items.Count > 0)
                    IsConnected = true;
            });
        }
    }
}