using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using LiteTube.Core.Common;
using LiteTube.Core.Common.Exceptions;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.Common.Tools;
using LiteTube.Core.Resources;
using LiteTube.Core.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiteTube.Core
{
    public class LiteTubeApp : Application
    {
        private static MainViewModel _viewModel = null;
        private static readonly ContainerBootstrapper _container = new ContainerBootstrapper();

        // Set to Home when the app is launched from Primary tile.
        // Set to DeepLink when the app is launched from Deep Link.
        private SessionType _sessionType = SessionType.None;

        // Set to true when the page navigation is being reset 
        bool _wasRelaunched = false;

        // set to true when 5 min passed since the app was relaunched
        bool _mustClearPagestack = false;

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (_viewModel == null)
                {
                    _container.Build();
                    _viewModel = new MainViewModel(() => _container.GetDataSource(), _container.ConnectionListener);
                }

                return _viewModel;
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static TransitionFrame RootFrame { get; protected set; }

        // prevents crash when trying to navigate to current page
        public static void NavigateTo(string url)
        {
            var frame = Current.RootVisual as TransitionFrame;
            if ((frame == null) || (url == frame.CurrentSource.ToString()))
                return;
            frame.Navigate(new Uri(url, UriKind.Relative));
        }

        // Code to execute when a contract activation such as a file open or save picker returns 
        // with the picked file or other return values
        protected void Application_ContractActivated(object sender,
            Windows.ApplicationModel.Activation.IActivatedEventArgs e)
        {
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        protected void Application_Launching(object sender, LaunchingEventArgs e)
        {
            ThemeManager.SetApplicationTheme(SettingsHelper.GetTheme());
            BuildLocalizedApplicationBar();

            // When a new instance of the app is launched, clear all deactivation settings
            RemoveCurrentDeactivationSettings();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        protected async void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // If some interval has passed since the app was deactivated (30 min),
            // then remember to clear the back stack of pages
            _mustClearPagestack = CheckDeactivationTimeStamp();

            // If IsApplicationInstancePreserved is not true, then set the session type to the value
            // saved in isolated storage. This will make sure the session type is correct for an
            // app that is being resumed after being tombstoned.
            if (!e.IsApplicationInstancePreserved)
            {
                RestoreSessionType();
            }

            LayoutHelper.InvokeFromUiThread(async () =>
            {
                if (!_mustClearPagestack)
                    return;

                NavigationHelper.GoHome();

                if (ViewModel.IsAuthorized)
                    await ViewModel.GetDataSource().LoginSilently(string.Empty);
            });

            ThemeManager.SetApplicationTheme(SettingsHelper.GetTheme());
            BuildLocalizedApplicationBar();

            // Ensure that application state is restored appropriately
            if (!ViewModel.IsDataLoaded)
            {
                await ViewModel.LoadData();
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        protected void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // When the applicaiton is deactivated, save the current deactivation settings to isolated storage
            SaveCurrentDeactivationSettings();
            PhoneApplicationService.Current.State["model"] = null;
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        protected void Application_Closing(object sender, ClosingEventArgs e)
        {
            PhoneApplicationService.Current.State["model"] = null;
            // When the application closes, delete any deactivation settings from isolated storage
            RemoveCurrentDeactivationSettings();
        }

        // Code to execute if a navigation fails
        protected void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        protected void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }

            e.Handled = true;

            if (e.ExceptionObject is LiteTubeException)
            {
                _container.DialogService.ShowError(AppResources.ErrorMessage);
                return;
            }

            if (e.ExceptionObject is PlaylistNotFoundException)
            {
                _container.DialogService.ShowError(AppResources.PlaylistNotFoundErrorMessage);
                return;
            }

            if (e.ExceptionObject.InnerException is System.Net.WebException)
                return;

#if DEBUG
#else
            if (e.ExceptionObject is GoogleApiException)
                return;

            if (e.ExceptionObject.InnerException is GoogleApiException)
                return;

            //убирем у пользователей, но оставим в дебаг
            if (e.ExceptionObject is UnauthorizedAccessException)
                return;
            
#endif
            _container.DialogService.ShowException(e.ExceptionObject);
        }

        private void Home_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoHome();
        }

        private void BuildLocalizedApplicationBar()
        {
            // Set the page's AplipcationBar to a new instance of ApplicationBar.
            var appBar = Application.Current.Resources["GlobalAppBar"] as ApplicationBar;
            var homeButton =
                ApplicationBarHelper.CreateApplicationBarIconButton("/Toolkit.Content/ApplicationBar.Home.png",
                    AppResources.Home, Home_Click);
            if (appBar == null)
                return;

            appBar.Buttons.Clear();
            appBar.MenuItems.Clear();
            appBar.Buttons.Add(homeButton);
        }

        // Called when the app is launched or closed. Removes all deactivation settings from
        // isolated storage
        private void RemoveCurrentDeactivationSettings()
        {
            SettingsHelper.ClearDeactivationSettings();
        }

        // Helper method to determine if the interval since the app was deactivated is
        // greater than 30 seconds
        private bool CheckDeactivationTimeStamp()
        {
            var lastDeactivated = SettingsHelper.GetDeactivateTime();
            var currentDuration = DateTimeOffset.Now.Subtract(lastDeactivated);
            return TimeSpan.FromSeconds(currentDuration.TotalSeconds) > TimeSpan.FromMinutes(20);
        }

        // Helper method to restore the session type from isolated storage.
        private void RestoreSessionType()
        {
            _sessionType = SettingsHelper.GetSessionType();
        }

        // Called when the app is deactivating. Saves the time of the deactivation and the 
        // session type of the app instance to isolated storage.
        private void SaveCurrentDeactivationSettings()
        {
            SettingsHelper.SaveDeactivateTime(DateTimeOffset.Now);
            SettingsHelper.SaveSessionType(_sessionType);
        }
    }
}