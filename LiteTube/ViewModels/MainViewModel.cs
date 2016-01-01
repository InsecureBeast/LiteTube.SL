using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.DataModel;
using LiteTube.Resources;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.ViewModels
{
    public class MainViewModel : SectionBaseViewModel
    {
        private readonly IDataSource _dataSource;
        private readonly MostPopularViewModel _mostPopularViewModel;
        private readonly ObservableCollection<VideoCategoryNodeViewModel> _categoryItems;
        private readonly ProfileSectionViewModel _profileSectionViewModel;

        public MainViewModel(IDataSource dataSource) : base(dataSource)
        {
            if (dataSource == null) 
                throw new ArgumentNullException("dataSource");
            
            _dataSource = dataSource;
            _mostPopularViewModel = new MostPopularViewModel(dataSource);
            _profileSectionViewModel = new ProfileSectionViewModel(dataSource);
            _categoryItems = new ObservableCollection<VideoCategoryNodeViewModel>();
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

        /// <summary>
        /// Creates and adds a few VideoItemViewModel objects into the Items collection.
        /// </summary>
        public async Task LoadData()
        {
            IsLoading = true;
            IsEmpty = false;
            
            await _mostPopularViewModel.FirstLoad();
            await LoadGuideCategories();

            IsDataLoaded = true;
            IsLoading = false;
            IsEmpty = !CategoryItems.Any();
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
    }
}