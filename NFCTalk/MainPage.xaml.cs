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

namespace NFCTalk
{
    public partial class MainPage : PhoneApplicationPage
    {
        NFCTalk.DataContext _dataContext = NFCTalk.DataContext.Singleton();
        ApplicationBarIconButton _settingsButton;
        ProgressIndicator _progressIndicator = new ProgressIndicator();

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
                _dataContext.Communication.Connect();

                _settingsButton.IsEnabled = true;
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _dataContext.Communication.Connected -= Connected;
            _dataContext.Communication.Connecting -= Connecting;
            _dataContext.Communication.ConnectionInterrupted -= ConnectionInterrupted;

            base.OnNavigatingFrom(e);
        }

        private void Connected()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                HideProgress();

                NavigationService.Navigate(new Uri("/TalkPage.xaml", UriKind.Relative));
            });
        }

        private void Connecting()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ShowProgress("Connecting...");

                _settingsButton.IsEnabled = false;
            });
        }

        private void ConnectionInterrupted()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                HideProgress();

                _dataContext.Communication.Disconnect();
                _dataContext.Communication.Connect();

                _settingsButton.IsEnabled = true;
            });
        }

        private void ShowProgress(String msg)
        {
            _progressIndicator.Text = msg;
            _progressIndicator.IsVisible = true;

            SystemTray.SetProgressIndicator(this, _progressIndicator);
        }

        private void HideProgress()
        {
            _progressIndicator.IsVisible = false;

            SystemTray.SetProgressIndicator(this, _progressIndicator);
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            _dataContext.Communication.Disconnect();

            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            _dataContext.Communication.Disconnect();

            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}