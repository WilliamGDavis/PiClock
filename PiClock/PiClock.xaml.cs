//Copyright William G Davis 2016
using PiClock.classes;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock
{
    //TODO: Write error handling for when the Apache server is inaccessable
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer DispatcherTimer { get; set; }
        private string _currentPin;
        private string CurrentPin
        {
            get { return _currentPin; }
            set
            {
                _currentPin = value;
                //Match the PIN length to the value in the Settings class and attempt to login if they match
                if (Settings.ConvertStringToInt(Settings.PinLength) == _currentPin.Length)
                { Login(); }
            }
        }

        public MainPage()
        {
            InitializeComponent();
            Settings.ReadLocalSettings(); //Read all settings values from "LocalSettings" and assign them to the static class Settings
            DispatcherTimerSetup(); //Display the current date/time in a readable format and create the ticking "timer" to update the date/time every second
            //textBlock_CurrentTime.Text = Format_dt_Current(TickingTimer.DispatcherTimerSetup());
            //Settings.EraseAllSettings();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        { textBlock_CurrentTime.Text = Format_dt_Current(DateTime.Now); }

        //Keypad Events
        private void btn_0_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_0.Content; }
        private void btn_1_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_1.Content; }
        private void btn_2_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_2.Content; }
        private void btn_3_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_3.Content; }
        private void btn_4_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_4.Content; }
        private void btn_5_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_5.Content; }
        private void btn_6_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_6.Content; }
        private void btn_7_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_7.Content; }
        private void btn_8_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_8.Content; }
        private void btn_9_Click(object sender, RoutedEventArgs e)
        { CurrentPin += btn_9.Content; }
        
        //Quick Peek Button Event
        private async void button_QuickPeek_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Employee> employeeList = await GetEmployeeList();

                //Stop the timer (not needed for the following page)
                DispatcherTimer.Stop();

                //Navigate to the next "QuickView" page and pass the employeeList object
                Frame.Navigate(typeof(QuickView), employeeList);
            }
            catch (Exception ex)
            { textBlock_Result.Text = ex.Message; }
        }

        public void DispatcherTimerSetup()
        {
            //Create a new timer and assign the Tick event
            DispatcherTimer = new DispatcherTimer();
            DispatcherTimer.Tick += DispatcherTimer_Tick;

            //Set how often the tick will happen
            DispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            //Start the Timer Function
            DispatcherTimer.Start();
        }

        void DispatcherTimer_Tick(object sender, object e)
        { textBlock_CurrentTime.Text = Format_dt_Current(DateTime.Now); }

        private static string Format_dt_Current(DateTime dt)
        {
            //Format the 'Main' current date/time into a friendly structure
            string newString = dt.ToString("dddd MMMM dd, yyyy");
            newString += "\n";
            newString += dt.ToString("hh:mm:sstt");
            return newString;
        }

        private async void Login()
        {
            var employee = new Employee();
            string result = null;
            try
            { result = await TryLogin(); }
            catch (HttpRequestException)
            { return; }

            //If a PIN is not in the database, the TryLogin() will return an empty array
            employee = JsonConvert.DeserializeObject<Employee>(result, new JsonSerializerSettings { Error = CommonMethods.HandleDeserializationError });

            if (null != employee)
            { //Successful Login
                CurrentPin = "";
                Frame.Navigate(typeof(EmployeePage), employee);
            }
            else
            { //Unsuccessful Login
                CurrentPin = "";
                textBlock_Result.Text = "Incorrect Login";
                return;
            }
        }

        private async Task<string> TryLogin()
        {
            try
            {
                var authentication = new Authentication();
                var paramDictionary = new Dictionary<string, string>();
                paramDictionary.Add("action", "PinLogin");
                paramDictionary.Add("pin", CurrentPin);
                authentication.ParamDictionary = paramDictionary;
                return await authentication.Login();
            }
            catch (HttpRequestException ex)
            { return ex.Message; }
        }

        private async Task<List<Employee>> GetEmployeeList()
        {
            string employeeList = await TryGetEmployeeList();
            if ("Cannot connect to database" != employeeList &&
                "[]" != employeeList) //Bad connection or an empty strin array returned from web service
            { return JsonConvert.DeserializeObject<List<Employee>>(employeeList); }
            else
            { return new List<Employee>(); }
        }

        private async Task<string> TryGetEmployeeList()
        {
            var employee = new Employee();
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "GetEmployeeList");
            employee.ParamDictionary = paramDictionary;
            return await employee.GetEmployeeList();
        }
    }
}
