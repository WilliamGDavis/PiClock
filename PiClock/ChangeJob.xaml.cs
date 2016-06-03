using PiClock.classes;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PiClock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangeJob : Page
    {
        Employee Employee { get; set; }

        public ChangeJob()
        { this.InitializeComponent(); }

        //Retrieve the data from the previous parent page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        { Employee = e.Parameter as Employee; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (null != Employee.CurrentJob)
            { textBox_CurrentJob.Text = Employee.CurrentJob.Description; }
            else
            { textBox_CurrentJob.Text = "None"; }
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(MainPage), null); }

        //Keypad Events
        private void btn_1_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_1.Content; }
        private void btn_2_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_2.Content; }
        private void btn_3_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_3.Content; }
        private void btn_4_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_4.Content; }
        private void btn_5_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_5.Content; }
        private void btn_6_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_6.Content; }
        private void btn_7_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_7.Content; }
        private void btn_8_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_8.Content; }
        private void btn_9_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_9.Content; }
        private void btn_0_Click(object sender, RoutedEventArgs e)
        { textBox_NewJob.Text += btn_0.Content; }

        private async void button_ChangeJob_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_CurrentJob.Text == textBox_NewJob.Text)
            {
                textBlock.Text = "You're already punched into this job!";
                textBox_NewJob.Text = "";
                return;
            }

            //Find the job the user is currently logged into
            //Settings settings = new Settings();
            string newJobId = await JobLookup();

            if ("null" == newJobId)
            {
                textBlock.Text = "Job Number does not exist!";
                textBox_NewJob.Text = "";
                return;
            }

            

            Punch punch = new Punch();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "PunchIn");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            punch.Employee = Employee;
            punch.ParamDictionary = paramDictionary;
            await punch.PunchIn();

            paramDictionary.Clear();
            paramDictionary.Add("action", "PunchIntoJob");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            paramDictionary.Add("currentJobId", (null != Employee.CurrentJob) ? Employee.CurrentJob.Id : "null"); //If a user is not currently logged into a job, set a null string value
            paramDictionary.Add("newJobId", newJobId);
            punch.Employee = Employee;
            punch.ParamDictionary = paramDictionary;
            await punch.PunchIntoJob();

            Frame.Navigate(typeof(MainPage), null);
        }


        private async Task<string> JobLookup()
        {
            Dictionary<string, string> paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "JobLookup");
            paramDictionary.Add("jobDescription", textBox_NewJob.Text);
            WebServiceCall wsCall = new WebServiceCall(Settings.ValidateSetting("UriPrefix"), paramDictionary);
            var httpResponse = new HttpResponseMessage();

            //Job Lookup by Description (Actually the JobCode)
            httpResponse = await wsCall.POST_JsonToWebApi();
            return await httpResponse.Content.ReadAsStringAsync();
        }
    }
}
