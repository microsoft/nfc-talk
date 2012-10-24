using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NFCTalk.Resources;
using Microsoft.Phone.Tasks;

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
        private ProgressIndicator _progressIndicator = new ProgressIndicator();

        public MainPage()
        {
            InitializeComponent();

            ApplicationBarMenuItem menuItem = new ApplicationBarMenuItem();
            menuItem.Text = "about";
            ApplicationBar.MenuItems.Add(menuItem);
            menuItem.Click += new EventHandler(aboutMenuItem_Click);

            DataContext = _dataContext;

            _settingsButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;

            _progressIndicator.IsIndeterminate = true;
        }

        /// <summary>
        /// If chat name has not been set yet (application has never been run) application
        /// automatically navigates to SettingsPage, otherwise listening to tap events is started.
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_dataContext.Settings.Name.Length == 0)
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            }
            else
            {
                _dataContext.Communication.Connected += Connected;
                _dataContext.Communication.Connecting += Connecting;
                _dataContext.Communication.ConnectionInterrupted += ConnectionInterrupted;
                _dataContext.Communication.UnableToConnect += UnableToConnect;
                _dataContext.Communication.Connect();

                _settingsButton.IsEnabled = true;
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Listening to tap events is stopped when navigating away from MainPage as
        /// we don't want to allow connecting while in SettingsPage or AboutPage, or while
        /// a connection is already established.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _dataContext.Communication.Connected -= Connected;
            _dataContext.Communication.Connecting -= Connecting;
            _dataContext.Communication.ConnectionInterrupted -= ConnectionInterrupted;
            _dataContext.Communication.UnableToConnect -= UnableToConnect;

            base.OnNavigatingFrom(e);
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
        /// Event handler to execute when connection is being made.
        /// </summary>
        private void Connecting()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ShowProgress("Connecting...");

                _settingsButton.IsEnabled = false;
            });
        }

        /// <summary>
        /// Event handler to execute when connection is interrupted.
        /// 
        /// Attempting to connect is restarted.
        /// </summary>
        private void ConnectionInterrupted()
        {
            _dataContext.Communication.Disconnect();
            _dataContext.Communication.Connect();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                HideProgress();

                _settingsButton.IsEnabled = true;
            });
        }

        /// <summary>
        /// Event handler to execute when attempting to connect fails.
        /// 
        /// Dialog asking to verify that a secondary bearer is available is displayed.
        /// </summary>
        private void UnableToConnect()
        {
            _dataContext.Communication.Disconnect();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                HideProgress();

                MessageBoxResult r = MessageBox.Show("Please make sure that Bluetooth has been turned on.", "Unable to connect", MessageBoxButton.OKCancel);

                if (r.HasFlag(MessageBoxResult.OK))
                {
                    ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
                    connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
                    connectionSettingsTask.Show();
                }
                else
                {
                    _dataContext.Communication.Connect();
                }
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
        private void settingsButton_Click(object sender, EventArgs e)
        {
            _dataContext.Communication.Disconnect();

            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Event handler to execute when about button has been clicked.
        /// 
        /// Attempting to connect is stopped and application navigates to AboutPage.
        /// </summary>
        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            _dataContext.Communication.Disconnect();

            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}