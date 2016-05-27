using Newtonsoft.Json;
using PiClock.classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace PiClock
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Employee_Info : Page
    {
        Employee Employee { get; set; }
        Dictionary<string, string> ParamDictionary { get; set; }
        List<Widget> RegularPunches { get; set; }
        List<Widget> JobPunches { get; set; }

        public Employee_Info()
        {
            InitializeComponent();
            RegularPunches = new List<Widget>();
            JobPunches = new List<Widget>();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        { Employee = e.Parameter as Employee; }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BuildWeeklyWidgets();
            cvs3.Source = RegularPunches;

            var punches = await TryGetTodaysPunches();
            if (null != punches)
            {
                BuildWidgets_TodaysPunches(punches);
            }

            if (null != punches.RegularPunchesOpen)
            {
                //Set a ticking Timer
            }


            cvs1.Source = RegularPunches;
            cvs2.Source = JobPunches;
            hubInfo.Header = string.Format("{0} {1}", Employee.fname, Employee.lname);
        }

        private void BuildWeeklyWidgets()
        {

            //WidgetList.Add(new Widget { Name = "Test1", Color = "Red" });
            //WidgetList.Add(new Widget { Name = "Test2", Color = "Blue" });
            //WidgetList.Add(new Widget { Name = "Test3", Color = "Red" });
            //WidgetList.Add(new Widget { Name = "Test4", Color = "Blue" });
        }

        private void BuildWidgets_TodaysPunches(EmployeePunches punches)
        {
            foreach (var punch in punches.RegularPunchesOpen)
            { BuildWidget_OpenRegularPunch(punch); }

            foreach (var punch in punches.RegularPunchesPaired)
            { BuildWidget_ClosedRegularPunch(punch); }

            foreach (var punch in punches.JobPunchesOpen)
            { BuildWidget_OpenJobPunch(punch); }
            
            foreach (var punch in punches.JobPunchesPaired)
            { BuildWidget_ClosedJobPunch(punch); }
        }

        private void BuildWidget_OpenRegularPunch(RegularPunchesOpen regularPunchOpen)
        {
            if (null != regularPunchOpen)
            {
                string punchIn = regularPunchOpen.TimeStamp.ToString("hh:mm tt");
                TimeSpan currentDuration = DateTime.Now.Subtract(regularPunchOpen.TimeStamp);
                RegularPunches.Add(new WidgetRegularPunchOpen
                {
                    Line1 = string.Format("IN: {0}", punchIn),
                    Line2 = string.Format("Duration: {0}", new DateTime(currentDuration.Ticks).ToString("HH:mm:ss")),
                    BackgroundColor = "SteelBlue",
                    TextColor = "White"
                });
            }
        }

        private void BuildWidget_ClosedRegularPunch(RegularPunchesPaired regularPunchPaired)
        {
            if (null != regularPunchPaired)
            {
                string punchIn = regularPunchPaired.TimeStampIn.ToString("hh:mm tt");
                string punchOut = regularPunchPaired.TimeStampOut.ToString("hh:mm tt");
                TimeSpan duration = regularPunchPaired.TimeStampOut.Subtract(regularPunchPaired.TimeStampIn);

                RegularPunches.Add(new WidgetRegularPunchPaired
                {
                    Line1 = string.Format("{0} - {1}", punchIn, punchOut),
                    Line2 = string.Format("Duration: {0}", duration),
                    BackgroundColor = "LightGray",
                    TextColor = "Black"
                });
            }
        }

        private void BuildWidget_OpenJobPunch(JobPunchesOpen jobPunchOpen)
        {
            if (null != jobPunchOpen)
            {
                string punchIn = jobPunchOpen.TimeStamp.ToString("hh:mm tt");
                TimeSpan currentDuration = DateTime.Now.Subtract(jobPunchOpen.TimeStamp);
                JobPunches.Add(new WidgetJobPunchOpen
                {
                    Line1 = string.Format("IN: {0}", punchIn),
                    Line2 = string.Format("Duration: {0}", new DateTime(currentDuration.Ticks).ToString("HH:mm:ss")),
                    BackgroundColor = "SteelBlue",
                    TextColor = "White"
                });
            }
        }

        private void BuildWidget_ClosedJobPunch(JobPunchesPaired jobPunchPaired)
        {
            if (null != jobPunchPaired)
            {
                string punchIn = jobPunchPaired.TimeStampIn.ToString("hh:mm tt");
                string punchOut = jobPunchPaired.TimeStampOut.ToString("hh:mm tt");
                TimeSpan duration = jobPunchPaired.TimeStampOut.Subtract(jobPunchPaired.TimeStampIn);

                JobPunches.Add(new WidgetJobPunchPaired
                {
                    Line1 = string.Format("{0} - {1}", punchIn, punchOut),
                    Line2 = string.Format("Duration: {0}", duration),
                    BackgroundColor = "LightGray",
                    TextColor = "Black"
                });
            }
        }

        //private void BuildTodayWidgetsJobPunch(JobPunchesPaired jobPunch = null)
        //{
        //    if (null != jobPunch)
        //    {
        //        string type = ("1" == jobPunch.Type) ? "IN" : "OUT";

        //        WidgetListJobPunchesPaired.Add(new WidgetJobPunchesPaired
        //        {
        //            Line1 = string.Format("{0} - {1}", type, jobPunch.IdJobs), //TODO: Convert Job ID to job description
        //            Line2 = string.Format("{0}", jobPunch.TimeStamp.ToString("hh:mm:ss tt"))
        //        });
        //    }
        //}

        private void button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        { Frame.Navigate(typeof(EmployeePage), Employee); }

        private async Task<EmployeePunches> TryGetTodaysPunches()
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "GetTodaysPunchesByEmployeeId");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            punch.Employee = Employee;
            punch.ParamDictionary = paramDictionary;
            var employeePunches = await punch.GetTodaysPunchesByEmployeeId();

            if (null != employeePunches)
            { return JsonConvert.DeserializeObject<EmployeePunches>(employeePunches);  }
            else
            { return null; }

        }

        private async Task<string> TryGetThisWeeksPunches()
        {
            return null;
        }
    }
}
