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

namespace NFCTalk
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        NFCTalk.DataContext _dataContext = NFCTalk.DataContext.Singleton();

        ApplicationBarIconButton _saveButton;
        ApplicationBarIconButton _cancelButton;

        public SettingsPage()
        {
            DataContext = _dataContext;

            InitializeComponent();

            _saveButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            _cancelButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_dataContext.Settings.Name.Length == 0)
            {
                NavigationService.RemoveBackEntry();
            }

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _saveButton.IsEnabled = nameInput.Text.Length > 0;
            _cancelButton.IsEnabled = _dataContext.Settings.Name.Length > 0;

            base.OnNavigatedTo(e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _dataContext.Settings.Name = nameInput.Text;

            NavigationService.GoBack();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void nameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _saveButton.IsEnabled = nameInput.Text.Length > 0;
        }
    }
}