using System;
using LiteTube.Common;
using System.Collections.ObjectModel;
using LiteTube.Common.Helpers;
using LiteTube.DataModel;
using LiteTube.Common.Tools;
using System.Collections.Generic;

namespace LiteTube.ViewModels
{
    class SettingsViewModel
    {
        private readonly ObservableCollection<string> _languages;
        private readonly ObservableCollection<string> _videoQualities;
        private readonly ObservableCollection<ApplicationTheme> _applicationThemes;
        private readonly Func<IDataSource> _getDataSource;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
        private string _selectedRegion;
        private string _selectedQuality;
        private ApplicationTheme _selectedApplicationTheme;
        private ApplicationTheme _oldSelectedApplicationTheme;

        public SettingsViewModel(Func<IDataSource> getGetDataSource, IConnectionListener connectionListener)
        {
            _getDataSource = getGetDataSource;
            _languages = new ObservableCollection<string>(I18nLanguages.Languages);
            var videoQuality = new VideoQuality();
            _videoQualities = new ObservableCollection<string>(videoQuality.GetQualityNames());

            _applicationThemes = new ObservableCollection<ApplicationTheme>(
                new List<ApplicationTheme>
                {
                    ApplicationTheme.Light,
                    ApplicationTheme.Dark
                });

            _navigatioPanelViewModel = new NavigationPanelViewModel(_getDataSource, connectionListener);
            _navigatioPanelViewModel.IsSettingsSelected = true;
            _selectedRegion = SettingsHelper.GetRegionName();
            _selectedQuality = SettingsHelper.GetQuality();
            _selectedApplicationTheme = SettingsHelper.GetTheme();
            _oldSelectedApplicationTheme = _selectedApplicationTheme;
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

        public ObservableCollection<ApplicationTheme> ApplicationThemes
        {
            get { return _applicationThemes; }
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

        public ApplicationTheme SelectedApplicationTheme
        {
            get { return _selectedApplicationTheme; }
            set
            {
                _selectedApplicationTheme = value;
#if SILVERLIGHT
                ThemeManager.SetApplicationTheme(_selectedApplicationTheme);
#endif
            }
        }

        public void Save()
        {
            SettingsHelper.SaveQuality(_selectedQuality);
            SettingsHelper.SaveRegion(_selectedRegion);
            SettingsHelper.SaveTheme(_selectedApplicationTheme);
            _getDataSource().Update(I18nLanguages.CheckRegionName(_selectedRegion), _selectedQuality);
        }

        public void Cancel()
        {
#if SILVERLIGHT
            ThemeManager.SetApplicationTheme(_oldSelectedApplicationTheme);
#endif
        }
    }
}
