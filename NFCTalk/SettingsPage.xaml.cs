using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
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
        private ApplicationBarIconButton _saveButton;
        private ApplicationBarIconButton _cancelButton;

        public SettingsPage()
        {
            DataContext = _dataContext;

            InitializeComponent();

            _saveButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            _cancelButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
        }

        /// <summary>
        /// If chat name has not beet set yet it means that application navigated to
        /// SettingsPage from the MainPage because the name was not set yet. In this
        /// case we consider the Settings page to be the start page of the application
        /// and do not wish to return to MainPage if user presses back key. Thus in this
        /// case MainPage is removed from the back stack before actually navigating back, 
        /// back to the previously used application that is.
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_dataContext.Settings.Name.Length == 0)
            {
                NavigationService.RemoveBackEntry();
            }

            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Save and cancel buttons are enabled only if chat name has been set. Otherwise
        /// we are encouraging the user to type in the chat name.
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _saveButton.IsEnabled = nameInput.Text.Length > 0;
            _cancelButton.IsEnabled = _dataContext.Settings.Name.Length > 0;

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Event handler to be executed when save button is clicked.
        /// 
        /// Saves the entered chat name to the application settings.
        /// </summary>
        private void saveButton_Click(object sender, EventArgs e)
        {
            _dataContext.Settings.Name = nameInput.Text;

            NavigationService.GoBack();
        }

        /// <summary>
        /// Event handler to be executed when cancel button is clicked.
        /// 
        /// Returns application to MainPage.
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Event handler to be executed when entered chat name changes.
        /// 
        /// Save button is enabled if a chat name has been entered, otherwise
        /// save button is disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _saveButton.IsEnabled = nameInput.Text.Length > 0;
        }
    }
}