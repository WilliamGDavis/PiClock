using PiClock.classes;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

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
                var currentJob = await TryGetCurrentJob();

                //If a job is returned, deserialize it into a Job object
                if ("[]" != currentJob && null != currentJob)
                {
                    Employee.CurrentJob = (Job)CommonMethods.Deserialize(typeof(Job), currentJob);
                    textBlock_CurrentPunch.Text = string.Format("Current Job: {0}", Employee.CurrentJob.Description);
                }
                else
                { textBlock_CurrentPunch.Text = "Current Job: None"; }
            }
            else
            { textBlock_CurrentPunch.Text = "Not Punched In"; }
        }

        #region Buttons
        private void button_Back_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(MainPage), null); }

        //Check to see if a user is currently logged in
        private async Task CheckLoginStatus()
        {
            var httpResponse = await Employee.CheckPunchedInStatus(Employee.id);
            var result = (string)CommonMethods.Deserialize(typeof(string), await httpResponse.Content.ReadAsStringAsync());
            LoggedIn = ("true" == result) ? true : false;
        }


        /**
        <summary>
        If Punch.PunchIn is successful, take the user back to the main page or the JobChange page, depending on wheter or not the "AllowPunchIntoJobWhenPunchingIn" setting is true or false
        If Punch.PunchIn is unsuccessful, display an error to the user (This is usually due to the employee already being punched in, which should be impossible currently)
        </summary>
        */
        private async void button_PunchIn_Click(object sender, RoutedEventArgs e)
        {
            var punchIn = await Punch.PunchIn(Employee.id.ToString());
            
            if (false == punchIn)
            {
                textBlock_CurrentPunch.Text = "An Error occured";
                return;
            }

            if ("true" == Settings.Read("AllowPunchIntoJobWhenPunchingIn"))
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

        #endregion

        //Check to see if a user is currently logged into a job, and parse it into a JSON string if they are
        private async Task<string> TryGetCurrentJob()
        {
            var httpResponse = await Job.GetCurrentJob(Employee.id.ToString());
            return await httpResponse.Content.ReadAsStringAsync();
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
