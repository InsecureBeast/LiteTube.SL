using System;
using LiteTube.Common;
using System.Collections.ObjectModel;
using LiteTube.DataModel;

namespace LiteTube.ViewModels
{
    class SettingsViewModel
    {
        private readonly ObservableCollection<string> _languages;
        private readonly ObservableCollection<string> _videoQualities;
        private readonly Func<IDataSource> _getDataSource;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
        private string _selectedRegion;
        private string _selectedQuality;

        public SettingsViewModel(Func<IDataSource> getGetDataSource, IConnectionListener connectionListener)
        {
            _getDataSource = getGetDataSource;
            _languages = new ObservableCollection<string>(I18nLanguages.Languages);
            var videoQuality = new VideoQuality();
            _videoQualities = new ObservableCollection<string>(videoQuality.GetQualityNames());
            _navigatioPanelViewModel = new NavigationPanelViewModel(_getDataSource, connectionListener);
            _navigatioPanelViewModel.IsSettingsSelected = true;
            _selectedRegion = SettingsHelper.GetRegionName();
            _selectedQuality = SettingsHelper.GetQuality();
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return _navigatioPanelViewModel; }
        }

        public ObservableCollection<string> Languages
        {
            get { return _languages; }
        }

        public ObservableCollection<string> VideoQualities
        {
            get { return _videoQualities; }
        }

        public string SelectedRegion
        {
            get { return _selectedRegion; }
            set { _selectedRegion = value; }
        }

        public string SelectedQuality
        {
            get { return _selectedQuality; }
            set { _selectedQuality = value; }
        }

        public void Save()
        {
            SettingsHelper.SaveQuality(_selectedQuality);
            SettingsHelper.SaveRegion(_selectedRegion);
            _getDataSource().Update(I18nLanguages.CheckRegionName(_selectedRegion), _selectedQuality);
        }
    }
}
