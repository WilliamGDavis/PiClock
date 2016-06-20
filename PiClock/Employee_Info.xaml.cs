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
    public sealed partial class Employee_Info : Page
    {
        Employee Employee { get; set; }
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
            //var todayPunches = await Punch.GetTodaysPunches(Employee.id);
            //if (null != todayPunches)
            //{ BuildWidgets_SingleDayPunches(todayPunches); }

            var todayPunches = await Punch.GetRangePunches(Employee.id);
            if (null != todayPunches)
            { BuildWidgets_SingleDayPunches(todayPunches); }

            var weeklyPunches = await Punch.GetThisWeeksPunches(Employee.id);
            if (null != weeklyPunches)
            { BuildWeeklyWidgets(weeklyPunches); }

            cvs1.Source = RegularPunches;
            cvs2.Source = JobPunches;
            cvs3.Source = ThisWeeksPunches;

            //TotalHours += calculateOpenPunchDuration(weeklyPunches).Duration();

            hubInfo.Header = string.Format("{0} {1}", Employee.fname, Employee.lname);
            hubSection_WeeklyTotal.Header = string.Format("Weekly Total: {0}:{1}:{2}", (int)TotalHours.TotalHours, TotalHours.Minutes, TotalHours.Seconds);
        }


        private void BuildWeeklyWidgets(EmployeePunchesByWeek weeklyPunches = null)
        {

            foreach (var day in weeklyPunches.WeekdayPunches)
            {
                double durationInSeconds = day.RegularPunches[0].TotalDurationInSeconds.GetValueOrDefault(); //Set the durationInSeconds to 0 if a null value is checked against
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
                    string stringDuration = new DateTime(duration.Ticks).ToString(string.Format("{0}:{1}:{2}", (int)duration.TotalHours, duration.Minutes, duration.Seconds));
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

        private void BuildWidgets_SingleDayPunches(EmployeePunchesByDay punches)
        {
            //BuildWidget_OpenRegularPunch(punches.RegularPunchesOpen);
            BuildWidget_ClosedRegularPunch(punches.RegularPunches);
            //BuildWidget_OpenJobPunch(punches.JobPunchesOpen);
            BuildWidget_ClosedJobPunch(punches.JobPunches);
        }

        private void BuildWidget_OpenRegularPunch(RegularPunchOpen regularPunchOpen)
        {
            if ("" != regularPunchOpen.OpenId)
            {
                DateTime punchIn = regularPunchOpen.PunchIn.GetValueOrDefault();
                TimeSpan currentDuration = DateTime.Now.Subtract(punchIn).Duration();
                string formattedDuration = string.Format("{0}:{1}:{2}", (int)currentDuration.TotalHours, currentDuration.Minutes, currentDuration.Seconds);
                string stringDuration = new DateTime(currentDuration.Ticks).ToString(formattedDuration);
                RegularPunches.Add(new WidgetRegularPunchOpen
                {
                    Line1 = string.Format("{0}", regularPunchOpen.PunchIn.GetValueOrDefault().ToString("dddd MM/dd")),
                    Line2 = string.Format("Punch In: {0}", punchIn.ToString("hh:mm tt")),
                    Line3 = string.Format("Duration: {0}", stringDuration),
                    BackgroundColor = "SteelBlue",
                    TextColor = "White"
                });
            }
        }

        private void BuildWidget_ClosedRegularPunch(RegularPunches regularPunchPaired)
        {
            foreach (var punch in regularPunchPaired.Punches)
            {
                TimeSpan duration = TimeSpan.FromSeconds(punch.DurationInSeconds.GetValueOrDefault());
                string stringDuration = new DateTime(duration.Ticks).ToString("HH:mm:ss");
                DateTime punchIn = punch.PunchIn;
                DateTime punchOut = punch.PunchOut.GetValueOrDefault();

                if (punchOut != DateTime.MinValue)
                {
                    RegularPunches.Add(new WidgetRegularPunch
                    {
                        Line1 = string.Format("Duration: {0}", stringDuration),
                        Line2 = string.Format("in: {0}", punchIn.ToString("MM/dd hh:mm t")),
                        Line3 = string.Format("out: {0}", punchOut.ToString("MM/dd hh:mm t")),
                        BackgroundColor = "LightGray",
                        TextColor = "Black"
                    });
                }
                else
                {
                    //Determine the "current" duration from when the employee punched in
                    TimeSpan currentDuration = DateTime.Now.Subtract(punchIn).Duration();
                    string formattedDuration = string.Format("{0}:{1}:{2}", (int)currentDuration.TotalHours, currentDuration.Minutes, currentDuration.Seconds);
                    stringDuration = new DateTime(currentDuration.Ticks).ToString(formattedDuration);
                    RegularPunches.Add(new WidgetRegularPunch
                    {
                        Line1 = string.Format("Duration: {0}", stringDuration),
                        Line2 = string.Format("in: {0}", punchIn.ToString("MM/dd hh:mm t")),
                        BackgroundColor = "SteelBlue",
                        TextColor = "White"
                    });
                }


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

        private void BuildWidget_ClosedJobPunch(JobPunches jobPunchPaired)
        {
            foreach (var punch in jobPunchPaired.Punches)
            {
                TimeSpan duration = TimeSpan.FromSeconds(punch.DurationInSeconds.GetValueOrDefault());
                string stringDuration = new DateTime(duration.Ticks).ToString("HH:mm:ss");
                DateTime punchIn = punch.PunchIn;
                DateTime punchOut = punch.PunchOut.GetValueOrDefault();

                if (punchOut != DateTime.MinValue)
                {
                    JobPunches.Add(new WidgetJobPunch
                    {
                        Line1 = string.Format("{0}", punch.JobInformation.Description),
                        Line2 = string.Format("in: {0}", punchIn.ToString("MM/dd hh:mm t")),
                        Line3 = string.Format("out: {0}", punchOut.ToString("MM/dd hh:mm t")),
                        BackgroundColor = "LightGray",
                        TextColor = "Black"
                    });
                }
                else
                {
                    JobPunches.Add(new WidgetJobPunch
                    {
                        Line1 = string.Format("{0}", punch.JobInformation.Description),
                        Line2 = string.Format("Punch In: {0}", punchIn.ToString("MM/dd hh:mm t")),
                        BackgroundColor = "SteelBlue",
                        TextColor = "White"
                    });
                }

            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(EmployeePage), Employee); }

        //private TimeSpan calculateOpenPunchDuration(EmployeePunchesByWeek weeklyPunches = null)
        //{
        //    //Make sure there was a weeklyPunches object passed in
        //    if (null == weeklyPunches)
        //    { return TimeSpan.Zero; }

        //    //If DateTime has a default value (usually when there is not an open punch)
        //    if (DateTime.MinValue == weeklyPunches.OpenPunches.RegularPunchesOpen.PunchIn.GetValueOrDefault())
        //    { return TimeSpan.Zero; }

        //    //Return the current time minus the time punched in as a TimeSpan
        //    return DateTime.Now.Subtract(weeklyPunches.OpenPunches.RegularPunchesOpen.PunchIn.GetValueOrDefault()).Duration();
        //}
    }
}
