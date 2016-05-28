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
            { BuildWidgets_TodaysPunches(todayPunches); }
            
            var weeklyPunches = await TryGetThisWeeksPunches();
            if (null != weeklyPunches)
            { BuildWeeklyWidgets(weeklyPunches); }

            cvs1.Source = RegularPunches;
            cvs2.Source = JobPunches;
            cvs3.Source = ThisWeeksPunches;

            hubInfo.Header = string.Format("{0} {1}", Employee.fname, Employee.lname);
            hubSection_WeeklyTotal.Header = string.Format("Weekly Total: {0}", new DateTime(TotalHours.Ticks).ToString("HH:mm:ss"));
        }

        private void BuildWeeklyWidgets(List<WeekdayPunch> weeklyPunches = null)
        {
            foreach (var day in weeklyPunches)
            {
                double durationInSeconds = day.DurationInSeconds.GetValueOrDefault(); //Set the durationInSeconds to 0 if a null value is checked against
                string backgroundColor = (day.DayName == DateTime.Now.DayOfWeek.ToString()) ? "SteelBlue" : "LightGray";
                string foregroundColor = (day.DayName == DateTime.Now.DayOfWeek.ToString()) ? "White" : "Black";
                //TODO: Check to see if the user is currently punched in (DO NOT match against the day of the week)
                if (0 == day.DurationInSeconds)
                {
                    //DateTime maxPunch = day.MaxPunch.GetValueOrDefault();
                    //DateTime minPunch = day.DurationInSeconds.GetValueOrDefault();
                    //TimeSpan duration = DateTime.Now.Subtract(TimeSpan.FromSeconds(day.DurationInSeconds));
                    
                    ThisWeeksPunches.Add(new Widget
                    {
                        Line1 = string.Format("{0}", day.DayName),
                        Line2 = string.Format("No Punches Available"),
                        BackgroundColor = backgroundColor,
                        TextColor = foregroundColor
                    });
                }

                if (0 != day.DurationInSeconds)
                {
                    //DateTime maxPunch = day.MaxPunch.GetValueOrDefault();
                    //DateTime minPunch = day.DurationInSeconds.GetValueOrDefault();
                    TimeSpan duration = TimeSpan.FromSeconds(durationInSeconds);
                    TotalHours += duration;
                    ThisWeeksPunches.Add(new Widget
                    {
                        Line1 = string.Format("{0}", day.DayName),
                        Line2 = string.Format("Duration: {0}", new DateTime(duration.Ticks).ToString("HH:mm:ss")),
                        BackgroundColor = backgroundColor,
                        TextColor = foregroundColor
                    });
                }

            }
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
                string punchIn = regularPunchOpen.DateTime.ToString("hh:mm tt");
                TimeSpan currentDuration = DateTime.Now.Subtract(regularPunchOpen.DateTime);
                RegularPunches.Add(new WidgetRegularPunchOpen
                {
                    Line1 = string.Format("{0}", DateTime.Now.ToString("dddd MM/dd")),
                    Line2 = string.Format("Punch In: {0}", punchIn),
                    Line3 = string.Format("Duration: {0}", new DateTime(currentDuration.Ticks).ToString("HH:mm:ss")),
                    BackgroundColor = "SteelBlue",
                    TextColor = "White"
                });
            }
        }

        private void BuildWidget_ClosedRegularPunch(RegularPunchesPaired regularPunchPaired)
        {
            if (null != regularPunchPaired)
            {
                string punchIn = regularPunchPaired.DateTimeIn.ToString("hh:mm tt");
                string punchOut = regularPunchPaired.DateTimeOut.ToString("hh:mm tt");
                TimeSpan duration = regularPunchPaired.DateTimeOut.Subtract(regularPunchPaired.DateTimeIn);

                RegularPunches.Add(new WidgetRegularPunchPaired
                {
                    Line1 = string.Format("{0}", DateTime.Now.ToString("dddd MM/dd")),
                    Line2 = string.Format("{0} - {1}", punchIn, punchOut),
                    Line3 = string.Format("Duration: {0}", duration),
                    BackgroundColor = "LightGray",
                    TextColor = "Black"
                });
            }
        }

        private void BuildWidget_OpenJobPunch(JobPunchesOpen jobPunchOpen)
        {
            if (null != jobPunchOpen)
            {
                string punchIn = jobPunchOpen.DateTime.ToString("hh:mm tt");
                TimeSpan currentDuration = DateTime.Now.Subtract(jobPunchOpen.DateTime);
                JobPunches.Add(new WidgetJobPunchOpen
                {
                    Line1 = string.Format("{0}", jobPunchOpen.JobDescription),
                    Line2 = string.Format("Punch In: {0}", punchIn),
                    Line3 = string.Format("Duration: {0}", new DateTime(currentDuration.Ticks).ToString("HH:mm:ss")),
                    BackgroundColor = "SteelBlue",
                    TextColor = "White"
                });
            }
        }

        private void BuildWidget_ClosedJobPunch(JobPunchesPaired jobPunchPaired)
        {
            if (null != jobPunchPaired)
            {
                string punchIn = jobPunchPaired.DateTimeIn.ToString("hh:mm tt");
                string punchOut = jobPunchPaired.DateTimeOut.ToString("hh:mm tt");
                TimeSpan duration = jobPunchPaired.DateTimeOut.Subtract(jobPunchPaired.DateTimeIn);

                JobPunches.Add(new WidgetJobPunchPaired
                {
                    Line1 = string.Format("{0}", jobPunchPaired.JobDescription),
                    Line2 = string.Format("{0} - {1}", punchIn, punchOut),
                    Line3 = string.Format("Duration: {0}", duration),
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

        private async Task<List<WeekdayPunch>> TryGetThisWeeksPunches()
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "GetThisWeeksPunchesByEmployeeId");
            paramDictionary.Add("employeeId", Employee.id.ToString());
            punch.Employee = Employee;
            punch.ParamDictionary = paramDictionary;
            var weeklyPunches = await punch.GetThisWeeksPunchesByEmployeeId();

            if (null != weeklyPunches)
            { return JsonConvert.DeserializeObject<List<WeekdayPunch>>(weeklyPunches, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include}); }
            else
            { return null; }
        }
    }
}
