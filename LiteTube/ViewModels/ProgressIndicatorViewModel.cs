using LiteTube.DataModel;
using LiteTube.Resources;
using Microsoft.Phone.Shell;
using System;

namespace LiteTube.ViewModels
{
    public class ProgressIndicatorViewModel : PropertyChangedBase
    {
        private ProgressIndicator _progressIndicator;
        private readonly Action<bool> _changeProgressIndicator;
        protected readonly NavigationPanelViewModel _navigatioPanelViewModel;

        public ProgressIndicatorViewModel(Func<IDataSource> getGeDataSource, IConnectionListener connectionListener, Action<bool> changeProgressIndicator)
        {
            _changeProgressIndicator = changeProgressIndicator;
            _navigatioPanelViewModel = new NavigationPanelViewModel(getGeDataSource, connectionListener);
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return _navigatioPanelViewModel; }
        }

        public ProgressIndicator ProgressIndicator
        {
            get { return _progressIndicator; }
            set
            {
                if (value == _progressIndicator)
                    return;

                _progressIndicator = value;
                NotifyOfPropertyChanged(() => ProgressIndicator);
            }
        }

        protected void ShowProgressIndicator()
        {
            var indicator = new ProgressIndicator
            {
                Text = AppResources.Loading,
                IsVisible = true,
                IsIndeterminate = true
            };

            ProgressIndicator = indicator;
            App.ViewModel.IndicatorHolder.ProgressIndicator = indicator;
            if (_changeProgressIndicator != null)
                _changeProgressIndicator(true);
        }

        protected void HideProgressIndicator()
        {
            ProgressIndicator = null;
            App.ViewModel.IndicatorHolder.ProgressIndicator = null;
            if (_changeProgressIndicator != null)
                _changeProgressIndicator(false);
        }
    }
}
