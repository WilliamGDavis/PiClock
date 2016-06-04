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
        List<Widget> ThisWeeksPunches { get; set; }
        TimeSpan TotalHours { get; set; }

        public Employee_Info()
        {
            InitializeComponent();
            RegularPunches = new List<Widget>();
            JobPunches = new List<Widget>();
            ThisWeeksPunches = new List<Widget>();
            TotalHours = TimeSpan.Zero;
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        { Employee = e.Parameter as Employee; }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var todayPunches = await TryGetTodaysPunches();
            if (null != todayPunches)
            { BuildWidgets_SingleDayPunches(todayPunches); }

            var weeklyPunches = await TryGetThisWeeksPunches();
            if (null != weeklyPunches)
            { BuildWeeklyWidgets(weeklyPunches); }

            cvs1.Source = RegularPunches;
            cvs2.Source = JobPunches;
            cvs3.Source = ThisWeeksPunches;

            hubInfo.Header = string.Format("{0} {1}", Employee.fname, Employee.lname);
            hubSection_WeeklyTotal.Header = string.Format("Weekly Total: {0}", new DateTime(TotalHours.Ticks).ToString("HH:mm:ss"));
        }

        private void BuildWeeklyWidgets(EmployeePunchesForTheWeek weeklyPunches = null)
        {
            foreach (var day in weeklyPunches.DayOfWeekPunch)
            {
                double durationInSeconds = day.RegularPunchesPaired[0].TotalDurationInSeconds.GetValueOrDefault(); //Set the durationInSeconds to 0 if a null value is checked against
                string backgroundColor = (day.DayName == DateTime.Now.DayOfWeek.ToString()) ? "SteelBlue" : "LightGray";
                string foregroundColor = (day.DayName == DateTime.Now.DayOfWeek.ToString()) ? "White" : "Black";
                TotalHours += TimeSpan.FromSeconds(durationInSeconds);
                
                //TODO: Check to see if the user is currently punched in and add it to the total duration
                if (0 == durationInSeconds)
                {
                    ThisWeeksPunches.Add(new Widget
                    {
                        Line1 = string.Format("{0}", day.DayName),
                        Line2 = string.Format("No Punches Available"),
                        BackgroundColor = backgroundColor,
                        TextColor = foregroundColor
                    });
                }

                if (0 != durationInSeconds)
                {
                    TimeSpan duration = TimeSpan.FromSeconds(durationInSeconds);
                    string stringDuration = new DateTime(duration.Ticks).ToString("HH:mm:ss");
                    //TotalHours += duration;
                    ThisWeeksPunches.Add(new Widget
                    {
                        Line1 = string.Format("{0}", day.DayName),
                        Line2 = string.Format("Duration: {0}", stringDuration),
                        BackgroundColor = backgroundColor,
                        TextColor = foregroundColor
                    });
                }
            }
        }

        private void BuildWidgets_SingleDayPunches(EmployeePunches punches)
        {
            BuildWidget_OpenRegularPunch(punches.RegularPunchesOpen);
            BuildWidget_ClosedRegularPunch(punches.RegularPunchesPaired);
            BuildWidget_OpenJobPunch(punches.JobPunchesOpen);
            BuildWidget_ClosedJobPunch(punches.JobPunchesPaired);
        }

        private void BuildWidget_OpenRegularPunch(RegularPunchOpen regularPunchOpen)
        {
            if ("" != regularPunchOpen.OpenId)
            {
                string punchIn = regularPunchOpen.PunchIn.GetValueOrDefault().ToString("hh:mm tt");
                TimeSpan currentDuration = DateTime.Now.Subtract(regularPunchOpen.PunchIn.GetValueOrDefault());
                string stringDuration = new DateTime(currentDuration.Ticks).ToString("HH:mm:ss");
                RegularPunches.Add(new WidgetRegularPunchOpen
                {
                    Line1 = string.Format("{0}", DateTime.Now.ToString("dddd MM/dd")),
                    Line2 = string.Format("Punch In: {0}", punchIn),
                    Line3 = string.Format("Duration: {0}", stringDuration),
                    BackgroundColor = "SteelBlue",
                    TextColor = "White"
                });
            }
        }

        private void BuildWidget_ClosedRegularPunch(RegularPunchPaired regularPunchPaired)
        {
            foreach (var punch in regularPunchPaired.Punches)
            {
                TimeSpan duration = TimeSpan.FromSeconds(punch.DurationInSeconds.GetValueOrDefault());
                string stringDuration = new DateTime(duration.Ticks).ToString("HH:mm:ss");
                string punchIn = punch.PunchIn.GetValueOrDefault().ToString("hh:mm tt");
                string punchOut = punch.PunchOut.GetValueOrDefault().ToString("hh:mm tt");

                RegularPunches.Add(new WidgetRegularPunchPaired
                {
                    Line1 = string.Format("{0}", DateTime.Now.ToString("dddd MM/dd")),
                    Line2 = string.Format("{0} - {1}", punchIn, punchOut),
                    Line3 = string.Format("Duration: {0}", stringDuration),
                    BackgroundColor = "LightGray",
                    TextColor = "Black"
                });
            
            }
        }

        private void BuildWidget_OpenJobPunch(JobPunchOpen jobPunchOpen)
        {
            if ("" != jobPunchOpen.OpenId)
            {
                string punchIn = jobPunchOpen.PunchIn.GetValueOrDefault().ToString("hh:mm tt");
                TimeSpan currentDuration = DateTime.Now.Subtract(jobPunchOpen.PunchIn.GetValueOrDefault());
                string stringDuration = new DateTime(currentDuration.Ticks).ToString("HH:mm:ss");

                JobPunches.Add(new WidgetJobPunchOpen
                {
                    Line1 = string.Format("{0}", jobPunchOpen.JobInformation.Description),
                    Line2 = string.Format("Punch In: {0}", punchIn),
                    Line3 = string.Format("Duration: {0}", stringDuration),
                    BackgroundColor = "SteelBlue",
                    TextColor = "White"
                });
            }
        }

        private void BuildWidget_ClosedJobPunch(JobPunchesPaired jobPunchPaired)
        {
            foreach (var punch in jobPunchPaired.Punches)
            {
                TimeSpan duration = TimeSpan.FromSeconds(punch.DurationInSeconds.GetValueOrDefault());
                string stringDuration = new DateTime(duration.Ticks).ToString("HH:mm:ss");
                string punchIn = punch.PunchIn.GetValueOrDefault().ToString("hh:mm tt");
                string punchOut = punch.PunchOut.GetValueOrDefault().ToString("hh:mm tt");

                JobPunches.Add(new WidgetJobPunchPaired
                {
                    Line1 = string.Format("{0}", punch.JobInformation.Description),
                    Line2 = string.Format("{0} - {1}", punchIn, punchOut),
                    Line3 = string.Format("Duration: {0}", stringDuration),
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

        private void button_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(EmployeePage), Employee); }

        private async Task<EmployeePunches> TryGetTodaysPunches()
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "GetSingleDayPunchesByEmployeeId");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            punch.Employee = Employee;
            punch.ParamDictionary = paramDictionary;
            var employeePunches = await punch.GetTodaysPunchesByEmployeeId();

            if (null != employeePunches)
            {
                return JsonConvert.DeserializeObject<EmployeePunches>(employeePunches, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
            }
            else
            { return null; }

        }

        private async Task<EmployeePunchesForTheWeek> TryGetThisWeeksPunches()
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "GetThisWeeksPunchesByEmployeeId");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            punch.Employee = Employee;
            punch.ParamDictionary = paramDictionary;
            var weeklyPunches = await punch.GetThisWeeksPunchesByEmployeeId();

            if (null != weeklyPunches)
            { return JsonConvert.DeserializeObject<EmployeePunchesForTheWeek>(weeklyPunches, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include}); }
            else
            { return null; }
        }
    }
}
