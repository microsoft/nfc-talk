using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Networking.Proximity;

namespace NFCTalk
{
    /// <summary>
    /// MainPage instructs user to tap other device to connect. If chat name has not been
    /// set yet application is automatically navigated to the SettingsPage to fill in the name.
    /// 
    /// Tapping other device navigates application to the TalkPage.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        private NFCTalk.DataContext _dataContext = NFCTalk.DataContext.Singleton;
        private ApplicationBarIconButton _settingsButton;
        private ApplicationBarIconButton _browseButton;
        private ApplicationBarMenuItem _aboutMenuItem = new ApplicationBarMenuItem();
        private ProgressIndicator _progressIndicator = new ProgressIndicator();

        public MainPage()
        {
            InitializeComponent();

            _aboutMenuItem.Text = "about";
            _aboutMenuItem.Click += new EventHandler(AboutMenuItem_Click);

            ApplicationBar.MenuItems.Add(_aboutMenuItem);

            DataContext = _dataContext;

            _settingsButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            _browseButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;

            _progressIndicator.IsIndeterminate = true;

            if (_dataContext.Communication.SupportsTriggeredDiscovery && _dataContext.Communication.SupportsBrowseDiscovery)
            {
                GuideTextBlock.Text = "Tap other device to connect directly or search for devices running this app and having Bluetooth enabled by clicking on the search icon. For tap to work, tap+send has to be turned on in the phone settings.";
            }
            else if (_dataContext.Communication.SupportsTriggeredDiscovery)
            {
                GuideTextBlock.Text = "Tap other device to connect.";
            }
            else if (_dataContext.Communication.SupportsBrowseDiscovery)
            {
                GuideTextBlock.Text = "Search for devices running this app and having Bluetooth enabled by clicking on the search icon.";
            }
            else
            {
                GuideTextBlock.Text = "Unfortunately it seems that your device does not support establishing proximity connections.";
            }
        }

        /// <summary>
        /// If chat name has not been set yet (application has never been run) application
        /// automatically navigates to SettingsPage, otherwise listening to tap events is started.
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _dataContext.Communication.Connected += Connected;
            _dataContext.Communication.Connecting += Connecting;
            _dataContext.Communication.ConnectivityProblem += ConnectivityProblem;
            _dataContext.Communication.Searching += Searching;
            _dataContext.Communication.SearchFinished += SearchFinished;

            if (_dataContext.PeerInformation != null)
            {
                _dataContext.Communication.Connect(_dataContext.PeerInformation);

                _dataContext.PeerInformation = null;
            }
            else
            {
                _settingsButton.IsEnabled = true;
                _browseButton.IsEnabled = _dataContext.Communication.SupportsBrowseDiscovery;
                _aboutMenuItem.IsEnabled = true;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            _dataContext.Communication.Connected -= Connected;
            _dataContext.Communication.Connecting -= Connecting;
            _dataContext.Communication.ConnectivityProblem -= ConnectivityProblem;
            _dataContext.Communication.Searching -= Searching;
            _dataContext.Communication.SearchFinished -= SearchFinished;
        }

        private void SearchFinished()
        {
            HideProgress();

            IReadOnlyList<PeerInformation> peers = _dataContext.Communication.Peers;

            if (peers != null && peers.Count > 0)
            {
                NavigationService.Navigate(new Uri("/PeersPage.xaml", UriKind.Relative));
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBoxResult r = MessageBox.Show("To be able to connect, please make sure that the other device is running NFC Talk.", "No peers found", MessageBoxButton.OK);

                    _settingsButton.IsEnabled = true;
                    _browseButton.IsEnabled = _dataContext.Communication.SupportsBrowseDiscovery;
                    _aboutMenuItem.IsEnabled = true;
                });
            }
        }

        /// <summary>
        /// Event handler to execute when connection is being made.
        /// </summary>
        private void Connecting()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ShowProgress("Connecting...");

                _settingsButton.IsEnabled = false;
                _browseButton.IsEnabled = false;
                _aboutMenuItem.IsEnabled = false;
            });
        }

        /// <summary>
        /// Event handler to execute when connection has been established.
        /// 
        /// Application is navigated to TalkPage.
        /// </summary>
        private void Connected()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                HideProgress();

                NavigationService.Navigate(new Uri("/TalkPage.xaml", UriKind.Relative));
            });
        }

        /// <summary>
        /// Event handler to execute when attempting to connect fails.
        /// 
        /// Dialog asking to verify that a secondary bearer is available is displayed.
        /// </summary>
        private void ConnectivityProblem()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                HideProgress();

                _settingsButton.IsEnabled = true;
                _browseButton.IsEnabled = _dataContext.Communication.SupportsBrowseDiscovery;
                _aboutMenuItem.IsEnabled = true;
            });
        }

        /// <summary>
        /// Event handler to execute when peers are being searched.
        /// </summary>
        private void Searching()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ShowProgress("Searching...");

                _settingsButton.IsEnabled = false;
                _browseButton.IsEnabled = false;
                _aboutMenuItem.IsEnabled = false;
            });
        }

        /// <summary>
        /// Show system tray progress indicator.
        /// </summary>
        /// <param name="msg">Text to show</param>
        private void ShowProgress(String msg)
        {
            _progressIndicator.Text = msg;
            _progressIndicator.IsVisible = true;

            SystemTray.SetProgressIndicator(this, _progressIndicator);
        }

        /// <summary>
        /// Hide system tray progress indicator.
        /// </summary>
        private void HideProgress()
        {
            _progressIndicator.IsVisible = false;

            SystemTray.SetProgressIndicator(this, _progressIndicator);
        }

        /// <summary>
        /// Event handler to execute when settings button has been clicked.
        /// 
        /// Attempting to connect is stopped and application navigates to SettingsPage.
        /// </summary>
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Event handler to execute when browse button has been clicked.
        /// 
        /// Attempts to search for nearby devices over Bluetooth.
        /// </summary>
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            _dataContext.Communication.Search();
        }

        /// <summary>
        /// Event handler to execute when about button has been clicked.
        /// 
        /// Attempting to connect is stopped and application navigates to AboutPage.
        /// </summary>
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}