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
            if (geDataSource == null) 
                throw new ArgumentNullException("geDataSource");

            _indicatorHolder = new ProgressIndicatorHolder();
            _mostPopularViewModel = new MostPopularViewModel(geDataSource, _connectionListener);
            _profileSectionViewModel = new ProfileSectionViewModel(geDataSource, connectionListener);
            _categoryItems = new ObservableCollection<VideoCategoryNodeViewModel>();
            _activitySectionViewModel = new ActivitySectionViewModel(geDataSource, _connectionListener);

            geDataSource().Subscribe((IListener<UpdateSettingsEventArgs>)this);
            geDataSource().Subscribe((IListener<UpdateContextEventArgs>)this);
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
            get { return SettingsHelper.IsContainsAuthorizationData(); }
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
            IsLoading = true;
            IsEmpty = false;

            await _mostPopularViewModel.FirstLoad();
            await LoadGuideCategories();
            if (_getGeDataSource().IsAuthorized)
                await _activitySectionViewModel.FirstLoad();

            IsDataLoaded = true;
            IsLoading = false;
            IsEmpty = !CategoryItems.Any();
        }

        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            WebAuthenticationResult result = args.WebAuthenticationResult;

            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                await GetGeDataSource().ContinueWebAuthentication(args, string.Empty);
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                //OutputToken("HTTP Error returned by AuthenticateAsync() : " + result.ResponseErrorDetail.ToString());
            }
            else
            {
                //OutputToken("Error returned by AuthenticateAsync() : " + result.ResponseStatus.ToString());
            }
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
            if (_getGeDataSource().IsAuthorized)
            {
                ActivitySectionViewModel.Items.Clear();
                await ActivitySectionViewModel.FirstLoad();
            }

            NotifyOfPropertyChanged(() => IsAuthorized);
        }

        public async void Notify(UpdateSettingsEventArgs e)
        {
            _mostPopularViewModel.Items.Clear();
            if (IsAuthorized)
                _activitySectionViewModel.Items.Clear();
            await LoadData();
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                IsConnected = e.IsConnected;

                if (e.IsConnected)
                {
                    await LoadData();
                    return;
                }

                if (CategoryItems.Count > 0)
                    IsConnected = true;
            });
        }
    }
}