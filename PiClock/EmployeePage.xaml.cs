using PiClock.classes;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.UI.Popups;

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
            textBlock.Text = string.Format("Welcome, {0} {1}", Employee.fname, Employee.lname);
            if (true == LoggedIn)
            {
                //This should return a JSON string ("null" or an array) or a null
                string currentJob = await TryGetCurrentJob();

                //If a job is returned, deserialize it into a Job object
                if ("[]" != currentJob && null != currentJob)
                {
                    Employee.CurrentJob = JsonConvert.DeserializeObject<Job>(currentJob);
                    textBlock_CurrentPunch.Text = string.Format("Current Job: {0}", Employee.CurrentJob.Description);
                }
                else
                { textBlock_CurrentPunch.Text = "Current Job: None"; }
            }
            else
            { textBlock_CurrentPunch.Text = "Not Punched In"; }
        }

        //Check to see if a user is currently logged in
        private async Task CheckLoginStatus()
        {
            if (true == await Employee.TryCheckLoginStatus(Employee.id))
            { LoggedIn = true; }
            else
            { LoggedIn = false; }
        }

        private void button_Back_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(MainPage), null); }

        private async void button_PunchIn_Click(object sender, RoutedEventArgs e)
        {
            //If a PunchIn is successful, take the user back to the main page or the JobChange page, depending on wheter or not the AllowPunchIntoJobWhenPunchingIn settings is true
            //If not, display an error
            if (false == await Punch.PunchIn(Employee.id.ToString()))
            {
                textBlock_CurrentPunch.Text = "An Error occured";
                return;
            }

            if("true" == Settings.Read("AllowPunchIntoJobWhenPunchingIn"))
            { Frame.Navigate(typeof(PunchIntoJobConfirmation), Employee); }
            else
            { Frame.Navigate(typeof(MainPage), null); }
        }

        //TODO: Consider whether or not to punch in a user if they clicked to punch into a job, but cancelled before typing in a job number
        private async void button_PunchOut_Click(object sender, RoutedEventArgs e)
        {
            string employeeId = Employee.id;
            //If a user is not currently logged into a job, set a null string value
            string currentJobId = (null != Employee.CurrentJob) ? Employee.CurrentJob.Id : "null";
            
            //If a PunchOut is successful, take the user back to the main page
            //If not, display an error
            if (true == await Punch.PunchOut(employeeId, currentJobId))
            { Frame.Navigate(typeof(MainPage), null); }
            else
            { textBlock_CurrentPunch.Text = "An Error occured"; }
        }

        private void button_ChangeJob_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(ChangeJob), Employee); }

        private void button_ViewInfo_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(Employee_Info), Employee); }

        //Check to see if a user is currently logged into a job, and parse it into a JSON string if they are
        private async Task<string> TryGetCurrentJob()
        {
            Job job = new Job();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "GetCurrentJob");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            job.Employee = Employee;
            job.ParamDictionary = paramDictionary;

            //Should return a JSON string or null (if there were errors)
            return await job.GetCurrentJob();
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
