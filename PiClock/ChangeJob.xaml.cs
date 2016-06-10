using Newtonsoft.Json;
using PiClock.classes;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PiClock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangeJob : Page
    {
        Employee Employee { get; set; }

        public ChangeJob()
        { InitializeComponent(); }

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
            //Check to make sure a user is not trying to punch into the job they're currently punched into
            if (textBox_CurrentJob.Text == textBox_NewJob.Text)
            {
                textBlock.Text = "You're already punched into this job!";
                textBox_NewJob.Text = "";
                return;
            }

            //Retrieve the JobId from the database that matches the new job description
            string jobDescription = textBox_NewJob.Text;
            string newJobId = await Punch.JobLookup(jobDescription);

            //Check to make sure that the new job exists in the database
            if ("false" == newJobId)
            {
                textBlock.Text = "Job Number does not exist!";
                textBox_NewJob.Text = "";
                return;
            }

            //Punch into the new job (and punch out of the old job)
            await Punch.PunchIntoNewJob(this.Employee, newJobId);
            Frame.Navigate(typeof(MainPage), null);
        }
    }
}
