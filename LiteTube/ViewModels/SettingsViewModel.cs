using System;
using LiteTube.Common;
using System.Collections.ObjectModel;
using LiteTube.Common.Helpers;
using LiteTube.DataModel;
using LiteTube.Common.Tools;
using System.Collections.Generic;
using System.Threading;

namespace LiteTube.ViewModels
{
    class SettingsViewModel : PropertyChangedBase
    {
        private readonly ObservableCollection<string> _regions;
        private readonly ObservableCollection<string> _videoQualities;
        private readonly ObservableCollection<ApplicationTheme> _applicationThemes;
        private readonly Func<IDataSource> _getDataSource;
        private readonly NavigationPanelViewModel _navigatioPanelViewModel;
        private string _selectedRegion;
        private string _selectedQuality;
        private ApplicationTheme _selectedApplicationTheme;
        private ApplicationTheme _oldSelectedApplicationTheme;
        private ObservableCollection<string> _applicationLanguages;
        private string _selectedLanguage;
        private bool _isMustRestarted = false;
        private string _currentLanguage;
        private bool _isAutoplayVideo = true;

        public SettingsViewModel(Func<IDataSource> getGetDataSource, IConnectionListener connectionListener)
        {
            _getDataSource = getGetDataSource;
            _regions = new ObservableCollection<string>(I18nLanguages.Languages);
            var videoQuality = new VideoQuality();
            _videoQualities = new ObservableCollection<string>(videoQuality.GetQualityNames());

            _applicationThemes = new ObservableCollection<ApplicationTheme>(ThemeManager.GetSupportedThemes());
            _applicationLanguages = new ObservableCollection<string>(LanguageManager.GetSupportedLanguages());

            _navigatioPanelViewModel = new NavigationPanelViewModel(_getDataSource, connectionListener);
            _navigatioPanelViewModel.IsSettingsSelected = true;
            _selectedRegion = SettingsHelper.GetRegionName();
            _selectedQuality = SettingsHelper.GetQuality();
            _selectedApplicationTheme = SettingsHelper.GetTheme();
            _selectedLanguage = SettingsHelper.GetLanguage();
            _currentLanguage = _selectedLanguage;
            _isAutoplayVideo = SettingsHelper.GetIsAutoPlayVideo();
            _oldSelectedApplicationTheme = _selectedApplicationTheme;
        }

        public NavigationPanelViewModel NavigationPanelViewModel
        {
            get { return _navigatioPanelViewModel; }
        }

        public ObservableCollection<string> Regions
        {
            get { return _regions; }
        }

        public ObservableCollection<string> ApplicationLanguages
        {
            get { return _applicationLanguages; }
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

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                IsMustRestarted = _selectedLanguage != _currentLanguage;
            }
        }

        public bool IsMustRestarted
        {
            get { return _isMustRestarted; }
            set
            {
                _isMustRestarted = value;
                NotifyOfPropertyChanged(() => IsMustRestarted);
            }
        }

        public bool IsAutoplayVideo
        {
            get { return _isAutoplayVideo; }
            set
            {
                _isAutoplayVideo = value;
                NotifyOfPropertyChanged(() => IsAutoplayVideo);
            }
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
            SettingsHelper.SaveLanguage(_selectedLanguage);
            SettingsHelper.SaveAutoplayVideo(_isAutoplayVideo);
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
