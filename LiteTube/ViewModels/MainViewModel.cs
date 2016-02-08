using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.Resources;
using LiteTube.ViewModels.Nodes;
using LiteTube.Common;
using Microsoft.Phone.Shell;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;

namespace LiteTube.ViewModels
{
    public class MainViewModel : SectionBaseViewModel, IListener<UpdateContextEventArgs>, IListener<UpdateSettingsEventArgs>
    {
        private readonly MostPopularViewModel _mostPopularViewModel;
        private readonly ObservableCollection<VideoCategoryNodeViewModel> _categoryItems;
        private readonly ProfileSectionViewModel _profileSectionViewModel;
        private readonly ActivitySectionViewModel _activitySectionViewModel;
        private readonly ProgressIndicatorHolder _indicatorHolder;

        public MainViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener)
            : base(geDataSource, connectionListener)
        {
            _indicatorHolder = new ProgressIndicatorHolder();
            _mostPopularViewModel = new MostPopularViewModel(_getGeDataSource, _connectionListener);
            _profileSectionViewModel = new ProfileSectionViewModel(_getGeDataSource, connectionListener);
            _categoryItems = new ObservableCollection<VideoCategoryNodeViewModel>();
            _activitySectionViewModel = new ActivitySectionViewModel(_getGeDataSource, _connectionListener);

            geDataSource().Subscribe((IListener<UpdateSettingsEventArgs>)this);
            geDataSource().Subscribe((IListener<UpdateContextEventArgs>)this);

            _hasItems = false;
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.About;
            }
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

        public ObservableCollection<VideoCategoryNodeViewModel> CategoryItems
        {
            get { return _categoryItems; }
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
            get { return _getGeDataSource().IsAuthorized; }
        }

        public IConnectionListener ConnectionListener
        {
            get { return _connectionListener; }
        }

        public ProgressIndicatorHolder IndicatorHolder
        {
            get { return _indicatorHolder; }
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

            _mostPopularViewModel.Items.Clear();
            await _mostPopularViewModel.FirstLoad();
            await LoadGuideCategories();
            if (_getGeDataSource().IsAuthorized)
            {
                _activitySectionViewModel.Items.Clear();
                await _activitySectionViewModel.FirstLoad();
            }

            IsDataLoaded = true;
            IsLoading = false;
            IsEmpty = !CategoryItems.Any();
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

            _categoryItems.Clear();
            foreach (var section in sections)
            {
                _categoryItems.Add(new VideoCategoryNodeViewModel(section));
            }
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            var viewModel = (VideoCategoryNodeViewModel)navObject.ViewModel;
            var categoryId = viewModel.CategoryId;
            var title = viewModel.Title;
            PhoneApplicationService.Current.State["model"] = new VideoCategorySectionViewModel(categoryId, title, _getGeDataSource, _connectionListener);
            App.NavigateTo("/SectionPage.xaml");
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
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                IsConnected = e.IsConnected;

                if (e.IsConnected)
                {
                    IsLoading = true;
                    IsEmpty = false;

                    await LoadGuideCategories();
                    if (_getGeDataSource().IsAuthorized)
                    {
                        _activitySectionViewModel.Items.Clear();
                        await _activitySectionViewModel.FirstLoad();
                    }
                    IsDataLoaded = true;
                    IsLoading = false;
                    IsEmpty = !CategoryItems.Any();
                    return;
                }

                if (CategoryItems.Count > 0)
                    IsConnected = true;
            });
        }
    }
}