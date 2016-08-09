using System;
using LiteTube.Core.DataModel;
using LiteTube.Core.Resources;
using Microsoft.Phone.Shell;
#if SILVERLIGHT

#endif

namespace LiteTube.Core.ViewModels
{
    public class ProgressIndicatorViewModel : PropertyChangedBase
    {
#if SILVERLIGHT
        private ProgressIndicator _progressIndicator;
#endif
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

#if SILVERLIGHT
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
#endif
        protected void ShowProgressIndicator()
        {
#if SILVERLIGHT
            var indicator = new ProgressIndicator
            {
                Text = AppResources.Loading,
                IsVisible = true,
                IsIndeterminate = true
            };

            ProgressIndicator = indicator;
            LiteTubeApp.ViewModel.IndicatorHolder.ProgressIndicator = indicator;
            if (_changeProgressIndicator != null)
                _changeProgressIndicator(true);
#endif
        }

        protected void HideProgressIndicator()
        {
#if SILVERLIGHT
            ProgressIndicator = null;
            LiteTubeApp.ViewModel.IndicatorHolder.ProgressIndicator = null;
            if (_changeProgressIndicator != null)
                _changeProgressIndicator(false);
#endif
        }

    }
    }
