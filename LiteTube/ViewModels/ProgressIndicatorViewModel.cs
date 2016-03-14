using LiteTube.Resources;
using Microsoft.Phone.Shell;
using System;

namespace LiteTube.ViewModels
{
    public class ProgressIndicatorViewModel : PropertyChangedBase
    {
        private ProgressIndicator _progressIndicator;
        private readonly Action<bool> _changeProgressIndicator;

        public ProgressIndicatorViewModel(Action<bool> changeProgressIndicator)
        {
            _changeProgressIndicator = changeProgressIndicator;
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
