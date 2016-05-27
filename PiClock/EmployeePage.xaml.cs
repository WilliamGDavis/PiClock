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

    public sealed partial class EmployeePage : Page
    {
        private Employee Employee { get; set; }
        private bool LoggedIn { get; set; }

        public EmployeePage()
        { InitializeComponent(); }

        //Retrieve the data from the previous parent page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        { Employee = e.Parameter as Employee; }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckLoginStatus();
            ShowOrHideButtons();
            textBlock.Text = String.Format("Welcome, {0} {1}", Employee.fname, Employee.lname);
            if (true == LoggedIn)
            {
                //This should return a JSON string ("null" or an array) or a null
                string currentJob = await TryCheckCurrentJob();

                //If a job is returned, deserialize it into a Job object
                if ("null" != currentJob && null != currentJob)
                {
                    Employee.CurrentJob = JsonConvert.DeserializeObject<Job>(currentJob);
                    textBlock_CurrentPunch.Text = String.Format("Current Job: {0}", Employee.CurrentJob.Description);
                }
                else
                { textBlock_CurrentPunch.Text = "Current Job: None"; }
            }
            else
            { textBlock_CurrentPunch.Text = "Not Logged In"; }
        }
        
        //Check to see if a user is currently logged in
        private async Task CheckLoginStatus()
        {
            if (true == await TryCheckLoginStatus())
            { LoggedIn = true; }
            else 
            { LoggedIn = false; }
        }

        private void button_Back_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(MainPage), null); }

        private async void button_PunchIn_Click(object sender, RoutedEventArgs e)
        {
            //Display a message box allowing a user to punch directly into a job number, or no job number
            //If a user chooses to punch into a job number, take them to the ChangeJob page
            //If not, punch them in and take them back to the Main screen
            if (true == await AskToPunchIntoJob())
            { Frame.Navigate(typeof(ChangeJob), Employee); }
            else
            {
                //If a PunchIn is successful, take the user back to the main page
                //If not, display an error
                if (true == await TryPunchIn())
                { Frame.Navigate(typeof(MainPage), null); }
                else
                {
                    textBlock_CurrentPunch.Text = "An Error occured";
                    return;
                }
            }
        }

        //TODO: Consider whether or not to punch in a user if they clicked to punch into a job, but cancelled before typing in a job number
        private async void button_PunchOut_Click(object sender, RoutedEventArgs e)
        {
            //If a PunchOut is successful, take the user back to the main page
            //If not, display an error
            if (true == await TryPunchOut())
            { Frame.Navigate(typeof(MainPage), null); }
            else
            { textBlock_CurrentPunch.Text = "An Error occured"; return; }
        }

        private void button_ChangeJob_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(ChangeJob), Employee); }

        private void button_ViewInfo_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(Employee_Info), Employee); }

        //Attempt to punch into the database
        private async Task<bool> TryPunchIn()
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "PunchIn");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            punch.Employee = Employee;
            punch.ParamDictionary = paramDictionary;
            if (null != await punch.PunchIn())
            { return true; }
            else
            { return false; }
        }

        //Attempt to punch out of the database
        private async Task<bool> TryPunchOut()
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "PunchOut");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            paramDictionary.Add("currentJobId", (null != Employee.CurrentJob) ? Employee.CurrentJob.Id : "null"); //If a user is not currently logged into a job, set a null string value
            punch.Employee = Employee;
            punch.ParamDictionary = paramDictionary;
            if (null != await punch.PunchOut())
            { return true; }
            else
            { return false; }
        }

        //Check to see if a user is currently logged into a job, and parse it into a JSON string if they are
        private async Task<string> TryCheckCurrentJob()
        {
            Job job = new Job();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "CheckCurrentJob");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            job.Employee = Employee;
            job.ParamDictionary = paramDictionary;

            //Should return a JSON string or null (if there were errors)
            return await job.CheckCurrentJob();
        }

        //Check the database to see if a user is logged in currently
        private async Task<bool> TryCheckLoginStatus()
        {
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "CheckLoginStatus");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            Employee.ParamDictionary = paramDictionary;

            //Should return true or false (will also return false if there was an error)
            return await Employee.CheckLoginStatus();
        }

        //Show a message box asking the user if they want to punch directly into a job after they punch in
        //If they hit no, it will just punch them into the system
        //If they hit yes, it will allow them to punch into a job
        private async Task<bool> AskToPunchIntoJob()
        {
            var dialogBox = new MessageDialog("");
            dialogBox.Title = "Punch Into a Job Number?";
            dialogBox.Commands.Add(new UICommand { Label = "Work on a Job", Id = 1 });
            dialogBox.Commands.Add(new UICommand { Label = "No", Id = 0 });
            var result = await dialogBox.ShowAsync();

            if (1 == (int)result.Id) //1 = Punch into a job, 0 = Do not punch into a job
            { return true; }
            else
            { return false; }
        }

        private void ShowOrHideButtons()
        {
            if (false == LoggedIn)
            {
                button_PunchIn.IsEnabled = true;
                button_PunchOut.IsEnabled = false;
                button_ChangeJob.IsEnabled = false;
            }
            else if (true == LoggedIn)
            {
                button_PunchIn.IsEnabled = false;
                button_PunchOut.IsEnabled = true;
                button_ChangeJob.IsEnabled = true;
            }
            else
            { return; }
        }

    }
}
