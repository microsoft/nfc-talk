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

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DataContext = _dataContext;

            ///Sample code to call helper function to localize the ApplicationBar
            //BuildApplicationBar();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_dataContext.Settings.Name.Length == 0)
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            }
            else
            {
                _dataContext.Communication.Connect();
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _dataContext.Communication.Disconnect();

            base.OnNavigatingFrom(e);
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void ConnectGuide_Tap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/TalkPage.xaml", UriKind.Relative));
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Images/appbar_button1.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.appbar_buttonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.appbar_menuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}