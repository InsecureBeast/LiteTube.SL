using LiteTube.DataModel;
using MyToolkit.Command;
using System.Windows.Input;
using System;
using System.Windows;

namespace LiteTube.ViewModels
{
    public class ProfileSectionViewModel : PropertyChangedBase
    {
        private readonly RelayCommand<FrameworkElement> _subscriptionsCommand;
        private readonly RelayCommand<FrameworkElement> _historyCommand;
        private readonly RelayCommand<FrameworkElement> _recommendedCommand;
        private readonly RelayCommand<FrameworkElement> _videoCategoryCommand;
        private readonly RelayCommand<FrameworkElement> _favoritesCommand;
        private readonly IDataSource _datasource;

        public ProfileSectionViewModel(IDataSource datasource)
        {
            _datasource = datasource;
            _subscriptionsCommand = new RelayCommand<FrameworkElement>(Subscriptions);
            _historyCommand = new RelayCommand<FrameworkElement>(GetHistory);
            _recommendedCommand = new RelayCommand<FrameworkElement>(Recommended);
            _videoCategoryCommand = new RelayCommand<FrameworkElement>(VideoCategories);
            _favoritesCommand = new RelayCommand<FrameworkElement>(Favorites);

            datasource.ContextUpdated += OnContextUpdated;
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

        public bool IsAuthorized
        {
            get { return _datasource.IsAuthorized; }
        }

        private void Recommended(FrameworkElement control)
        {
            //var page = VisualHelper.FindParent<Page>(control);
            //if (page == null)
            //    return;

            //page.Frame.Navigate(typeof(PivotPage), new PivotPageViewModel(0, _datasource));
        }

        private void Subscriptions(FrameworkElement control)
        {
            //var page = VisualHelper.FindParent<Page>(control);
            //if (page == null)
            //    return;

            //page.Frame.Navigate(typeof(PivotPage), new PivotPageViewModel(1, _datasource));
        }

        private void GetHistory(FrameworkElement control)
        {
            //var page = VisualHelper.FindParent<Page>(control);
            //if (page == null)
            //    return;

            //page.Frame.Navigate(typeof(PivotPage), new PivotPageViewModel(3, _datasource));
        }

        private void VideoCategories(FrameworkElement control)
        {
            //var page = VisualHelper.FindParent<Page>(control);
            //if (page == null)
            //    return;

            //var index = IsAuthorized ? 4 : 0;
            //page.Frame.Navigate(typeof(PivotPage), new PivotPageViewModel(index, _datasource));
        }

        private void Favorites(FrameworkElement control)
        {
            //var page = VisualHelper.FindParent<Page>(control);
            //if (page == null)
            //    return;
            ////var index = IsAuthorized ? 2 : 0; //TODO favorites saved on device!!
            //page.Frame.Navigate(typeof(PivotPage), new PivotPageViewModel(2, _datasource));
        }

        private void OnContextUpdated(object sender, EventArgs e)
        {
            NotifyOfPropertyChanged(() => IsAuthorized);
        }
    }
}
