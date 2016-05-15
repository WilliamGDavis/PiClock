using PiClock.classes;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PiClock
{

    public sealed partial class LoggedIn : Page
    {
        private Employee employee;
        private bool loggedIn;

        public LoggedIn()
        {
            this.InitializeComponent();
            this.loggedIn = true;
            CheckLoggedIn();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Retrieve the data from the previous parent page
            //Note: You cannot set control values until the "Loaded" event fires
            this.employee = e.Parameter as Employee;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            textBlock.Text = String.Format("Welcome, {0} {1}", this.employee.fname, this.employee.lname); //Populate the Date & Time
            //if (true == this.loggedIn)
            //{
            //    var localSettings = ApplicationData.Current.LocalSettings;
            //    string Uri = string.Format("http://{0}:{1}{2}action=get_current_job_number&id={3}", localSettings.Values["DBLocation"],
            //                                                                                        localSettings.Values["SslPort"],
            //                                                                                        localSettings.Values["ApiLocation"],
            //                                                                                        employee.id);

            //    //Pull down the user's most recent OPEN punch
            //    string JsonString = await JsonMethods.GetCurrentJobNumber(Uri);

            //    if ("null" != JsonString){
            //        Punch punch = JsonConvert.DeserializeObject<Punch>(JsonString);

            //        //Check to see if the user is punched into a job, or just punched in
            //        if ("" != punch.id_jobs) //Punched into a job
            //        { textBlock_CurrentPunch.Text = string.Format("Current Job: {0}", punch.id_jobs); }
            //        else //Just punched in
            //        { textBlock_CurrentPunch.Text = "Current Job: N/A"; }
            //    }
            //}
        }

        private void button_Back_Click(object sender, RoutedEventArgs e)
        {
            //Go Back to the main page (MainPage)
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private void CheckLoggedIn()
        {
            if (true == this.loggedIn)
            {
                button_PunchIn.Visibility = Visibility.Collapsed;
                button_PunchOut.Visibility = Visibility.Visible;
            }
            else if (false == this.loggedIn)
            {
                button_PunchIn.Visibility = Visibility.Visible;
                button_PunchOut.Visibility = Visibility.Collapsed;
            }
            else
            {
                button_PunchIn.Visibility = Visibility.Visible;
                button_PunchOut.Visibility = Visibility.Visible;
            }
        }

        private void button_PunchIn_Click(object sender, RoutedEventArgs e)
        {
            this.loggedIn = false;
            StringBuilder dateTimeString = new StringBuilder();
            DateTime dt = DateTime.Now;
            string currentDateTime = dt.ToString();
            dateTimeString.Append("employeeId: " + employee.id.ToString());
            dateTimeString.Append(" | ");
            dateTimeString.Append("punchInTime: " + currentDateTime);
            
            string employeeId = employee.id.ToString();

            textBlock_CurrentPunch.Text = dateTimeString.ToString();
            
            button_PunchIn.Visibility = Visibility.Collapsed;
            button_PunchOut.Visibility = Visibility.Visible;
        }

        private void button_PunchOut_Click(object sender, RoutedEventArgs e)
        {
            this.loggedIn = false;
            StringBuilder dateTimeString = new StringBuilder();
            DateTime dt = DateTime.Now;
            string currentDateTime = dt.ToString();
            dateTimeString.Append("employeeId: " + employee.id.ToString());
            dateTimeString.Append(" | ");
            dateTimeString.Append("punchOutTime: " + currentDateTime);

            string employeeId = employee.id.ToString();

            textBlock_CurrentPunch.Text = dateTimeString.ToString();

            button_PunchIn.Visibility = Visibility.Visible;
            button_PunchOut.Visibility = Visibility.Collapsed;
        }

        private void button_ChangeJob_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChangeJob), null);
        }

        private void button_ViewInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PunchIn()
        {
            Settings settings = new Settings();
            Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            WebServiceCall wsCall = new WebServiceCall();
            wsCall.Uri = settings.ValidateSetting("UriPrefix");
            ParamDictionary.Add("action", "PunchIn");
            ParamDictionary.Add("employeeId", employee.id.ToString());
            ParamDictionary.Add("timestamp", null); //Timestamped automatically in MySql
            ParamDictionary.Add("type", "in");
            ParamDictionary.Add("open_status", "true");
            wsCall.ParamDictionary = ParamDictionary;

            await wsCall.POST_JsonToWebApi();
        }

        private async Task<string> PunchOut()
        {
            return null;
        }
    }
}
