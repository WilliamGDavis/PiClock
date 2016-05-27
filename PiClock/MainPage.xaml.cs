using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PiClock
{
    /// <summary>
    /// A launcher to allow a user to either configure settings or launch the timeclock
    /// </summary>
    public sealed partial class Launcher : Page
    {
        public Launcher()
        { InitializeComponent(); }

        //Configuration Button Event
        private void button_Config_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(Configuration), null); }

        //Timeclock Button Event
        private void button_TimeClock_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(MainPage), null); }
    }
}
