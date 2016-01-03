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
        private readonly IDataSource _dataSource;
        private readonly MostPopularViewModel _mostPopularViewModel;
        private readonly ObservableCollection<VideoCategoryNodeViewModel> _categoryItems;
        private readonly ProfileSectionViewModel _profileSectionViewModel;
        private readonly ActivitySectionViewModel _activitySectionViewModel;


        public MainViewModel(IDataSource dataSource) : base(dataSource)
        {
            if (dataSource == null) 
                throw new ArgumentNullException("dataSource");
            
            _dataSource = dataSource;
            _mostPopularViewModel = new MostPopularViewModel(dataSource);
            _profileSectionViewModel = new ProfileSectionViewModel(dataSource);
            _categoryItems = new ObservableCollection<VideoCategoryNodeViewModel>();
            _activitySectionViewModel = new ActivitySectionViewModel(dataSource);

            _dataSource.Subscribe((IListener<UpdateSettingsEventArgs>)this);
            _dataSource.Subscribe((IListener<UpdateContextEventArgs>)this);
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
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
            get { return !string.IsNullOrEmpty(SettingsHelper.GetUserId()); }
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
            if (_dataSource.IsAuthorized)
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
                await DataSource.ContinueWebAuthentication(args, string.Empty);
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
            var sections = await _dataSource.GetCategories();
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
            PhoneApplicationService.Current.State["model"] = new VideoCategorySectionViewModel(categoryId, title, _dataSource);
            App.NavigateTo("/SectionPage.xaml");
        }

        public async void Notify(UpdateContextEventArgs e)
        {
            if (_dataSource.IsAuthorized)
            {
                ActivitySectionViewModel.Items.Clear();
                await ActivitySectionViewModel.FirstLoad();
            }

            NotifyOfPropertyChanged(() => IsAuthorized);
        }

        public async void Notify(UpdateSettingsEventArgs e)
        {
            MostPopularViewModel.Items.Clear();
            await MostPopularViewModel.FirstLoad();
            await LoadData();
        }
    }
}