/*
 * Copyright © 2012-2013 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace NFCTalk
{
    /// <summary>
    /// SettingsPage enables editing the chat name for this device.
    /// </summary>
    public partial class SettingsPage : PhoneApplicationPage
    {
        private NFCTalk.DataContext _dataContext = NFCTalk.DataContext.Singleton;
        private ProgressIndicator _progressIndicator = new ProgressIndicator();
        private ApplicationBarIconButton _saveButton;

        public SettingsPage()
        {
            DataContext = _dataContext;

            InitializeComponent();

            _saveButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
        }

        /// <summary>
        /// Event handler to be executed when save button is clicked.
        /// 
        /// Saves the entered chat name to the application settings.
        /// </summary>
        private void saveButton_Click(object sender, EventArgs e)
        {
            _dataContext.Settings.Name = nameInput.Text;

            _dataContext.Communication.Stop();
            _dataContext.Communication.Start();

            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _dataContext.Communication.Connected += Connected;
            _dataContext.Communication.Connecting += Connecting;
            _dataContext.Communication.ConnectivityProblem += ConnectivityProblem;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            _dataContext.Communication.Connected -= Connected;
            _dataContext.Communication.Connecting -= Connecting;
            _dataContext.Communication.ConnectivityProblem -= ConnectivityProblem;
        }

        /// <summary>
        /// Event handler to execute when connection is being made.
        /// </summary>
        private void Connecting()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ShowProgress("Connecting...");
            });
        }

        /// <summary>
        /// Event handler to execute when connection has been established.
        /// 
        /// Application is navigated to TalkPage.
        /// </summary>
        private void Connected()
        {
            Dispatcher.BeginInvoke(() =>
            {
                HideProgress();

                NavigationService.Navigate(new Uri("/TalkPage.xaml", UriKind.Relative));
            });
        }
        
        /// <summary>
        /// Event handler to execute when attempting to connect fails.
        /// </summary>
        private void ConnectivityProblem()
        {
            Dispatcher.BeginInvoke(() =>
            {
                HideProgress();
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
    }
}