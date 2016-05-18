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
using Windows.UI.Popups;
using System.Net.Http;

namespace PiClock
{

    public sealed partial class LoggedIn : Page
    {
        private Employee employee;
        private bool loggedIn;

        public LoggedIn()
        { this.InitializeComponent(); }

        //Retrieve the data from the previous parent page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            employee = e.Parameter as Employee;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckLoginStatus();
            textBlock.Text = String.Format("Welcome, {0} {1}", employee.fname, employee.lname); //Populate the Date & Time
            if (true == loggedIn)
            {
                //Settings settings = new Settings();
                Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
                WebServiceCall wsCall = new WebServiceCall();
                HttpResponseMessage httpResponse = new HttpResponseMessage();
                wsCall.Uri = Settings.ValidateSetting("UriPrefix");
                ParamDictionary.Add("action", "CheckCurrentJob");
                ParamDictionary.Add("employeeId", employee.id.ToString());
                wsCall.ParamDictionary = ParamDictionary;

                httpResponse = await wsCall.POST_JsonToWebApi();
                string jobString = await httpResponse.Content.ReadAsStringAsync();
                //If a job is returned, deserialize it into a Job object
                if ("null" != jobString)
                {
                    employee.CurrentJob = JsonConvert.DeserializeObject<Job>(jobString);
                    textBlock_CurrentPunch.Text = String.Format("Current Job: {0}", employee.CurrentJob.Description);
                }
                else
                { textBlock_CurrentPunch.Text = "Current Job: None"; }

            }
            else
            { textBlock_CurrentPunch.Text = "Not Logged In"; }
        }

        //TODO: Refactor
        private async Task CheckLoginStatus()
        {
            Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            WebServiceCall wsCall = new WebServiceCall();
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            wsCall.Uri = Settings.ValidateSetting("UriPrefix");
            ParamDictionary.Add("action", "CheckLoginStatus");
            ParamDictionary.Add("employeeId", employee.id.ToString());
            wsCall.ParamDictionary = ParamDictionary;

            httpResponse = await wsCall.POST_JsonToWebApi();
            if ("true" == await httpResponse.Content.ReadAsStringAsync())
            {
                loggedIn = true;
                button_PunchIn.Visibility = Visibility.Collapsed;
                button_PunchOut.Visibility = Visibility.Visible;
                button_ChangeJob.Visibility = Visibility.Visible;
            }
            else if ("false" == await httpResponse.Content.ReadAsStringAsync())
            {
                loggedIn = false;
                button_PunchIn.Visibility = Visibility.Visible;
                button_PunchOut.Visibility = Visibility.Collapsed;
                button_ChangeJob.Visibility = Visibility.Collapsed;
            }
            else
            { return; }
        }

        private void button_Back_Click(object sender, RoutedEventArgs e)
        {
            //Go Back to the main page (MainPage)
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private async void button_PunchIn_Click(object sender, RoutedEventArgs e)
        {
            loggedIn = false;
            StringBuilder dateTimeString = new StringBuilder();
            DateTime dt = DateTime.Now;
            string currentDateTime = dt.ToString();
            dateTimeString.Append("employeeId: " + employee.id.ToString());
            dateTimeString.Append(" | ");
            dateTimeString.Append("punchInTime: " + currentDateTime);

            string employeeId = employee.id.ToString();

            textBlock_CurrentPunch.Text = dateTimeString.ToString();

            var dialogBox = new MessageDialog("", "Are You Working on a Job Number?");
            //dialogBox.Title = "Punch Into a Job Number?";
            dialogBox.Commands.Add(new UICommand { Label = "Work on a Job", Id = 0 });
            dialogBox.Commands.Add(new UICommand { Label = "No", Id = 1 });
            var result = await dialogBox.ShowAsync();

            button_PunchIn.Visibility = Visibility.Collapsed;
            button_PunchOut.Visibility = Visibility.Visible;
            button_ChangeJob.Visibility = Visibility.Visible;

            //If a user chooses to punch into a job number, take them to that page
            //If not, take them back to the Main screen
            if (0 == (int)result.Id)
            { Frame.Navigate(typeof(ChangeJob), employee); }
            else
            {
                var punch = new Punch();
                textBlock_CurrentPunch.Text = await punch.PunchIn(employee);
                Frame.Navigate(typeof(MainPage), null);
            }
        }

        private async void button_PunchOut_Click(object sender, RoutedEventArgs e)
        {
            this.loggedIn = false;
            StringBuilder dateTimeString = new StringBuilder();
            DateTime dt = DateTime.Now;
            var punch = new Punch();
            string currentDateTime = dt.ToString();
            dateTimeString.Append("employeeId: " + employee.id.ToString());
            dateTimeString.Append(" | ");
            dateTimeString.Append("punchOutTime: " + currentDateTime);

            string employeeId = employee.id.ToString();

            textBlock_CurrentPunch.Text = dateTimeString.ToString();
            
            textBlock_CurrentPunch.Text = await punch.PunchOut(employee);
            button_PunchIn.Visibility = Visibility.Visible;
            button_PunchOut.Visibility = Visibility.Collapsed;
            button_ChangeJob.Visibility = Visibility.Collapsed;

            Frame.Navigate(typeof(MainPage), null);
        }

        private void button_ChangeJob_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(ChangeJob), employee); }

        private void button_ViewInfo_Click(object sender, RoutedEventArgs e)
        { }
    }
}
