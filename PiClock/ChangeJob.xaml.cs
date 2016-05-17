using PiClock.classes;
using System.Collections.Generic;
using System.Net.Http;
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
        Employee employee { get; set; }

        public ChangeJob()
        { this.InitializeComponent(); }

        //Retrieve the data from the previous parent page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        { employee = e.Parameter as Employee; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (null != employee.CurrentJob)
            { textBox_CurrentJob.Text = employee.CurrentJob.Description; }
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
            //Find the job the user is currently logged into
            Settings settings = new Settings();
            Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            WebServiceCall wsCall = new WebServiceCall();
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            wsCall.Uri = settings.ValidateSetting("UriPrefix");

            //Job Lookup by Description (Actually the JobCode)
            ParamDictionary.Add("action", "JobLookup");
            ParamDictionary.Add("jobDescription", textBox_NewJob.Text);
            wsCall.ParamDictionary = ParamDictionary;
            httpResponse = await wsCall.POST_JsonToWebApi();
            string result = await httpResponse.Content.ReadAsStringAsync();

            if ("null" == result)
            {
                textBlock.Text = "Job Number does not exist!";
                return;
            }

            ParamDictionary.Clear();
            wsCall.ParamDictionary = null;

            //If a user is not currently logged into a job
            if (null == employee.CurrentJob)
            {
                //Create a new job punch
                ParamDictionary.Add("action", "JobPunch");
                ParamDictionary.Add("employeeId", employee.id.ToString());
                ParamDictionary.Add("newJobId", result);
                wsCall.ParamDictionary = ParamDictionary;
            }
            else
            {
                //Change the User's Job if they're currently logged into one
                ParamDictionary.Add("action", "ChangeJob");
                ParamDictionary.Add("employeeId", employee.id.ToString());
                ParamDictionary.Add("jobId", employee.CurrentJob.Id); 
                ParamDictionary.Add("newJobId", result);
                wsCall.ParamDictionary = ParamDictionary;
            }


            httpResponse = await wsCall.POST_JsonToWebApi();
            Frame.Navigate(typeof(MainPage), null);
        }


    }
}
