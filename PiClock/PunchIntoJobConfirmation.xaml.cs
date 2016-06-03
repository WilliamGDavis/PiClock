using PiClock.classes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// This page is only a temporary page while waiting for the UI.Popup class to be implemented into IoT

namespace PiClock
{
    public sealed partial class PunchIntoJobConfirmation : Page
    {
        Employee Employee { get; set; }

        public PunchIntoJobConfirmation()
        { InitializeComponent(); }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        { Employee = e.Parameter as Employee; }

        private void button_Yes_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        { Frame.Navigate(typeof(ChangeJob), Employee); }

        private void button_No_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        { Frame.Navigate(typeof(MainPage), null); }
    }
}
