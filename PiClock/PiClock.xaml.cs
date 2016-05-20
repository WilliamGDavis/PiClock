//Copyright William G Davis 2016
using PiClock.classes;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Net.Http;

namespace PiClock
{
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
                //Match the PIN length to the value in the Settings class
                if (Settings.ConvertStringToInt(Settings.PinLength) == _currentPin.Length)
                { TryLogin(); }
            }
        }

        public MainPage()
        {
            InitializeComponent();
            Settings.ReadAllSettings(); //Read all settings values from "LocalSettings" and assign them to the static class Settings
            //PinLength = Settings.ConvertStringToInt(Settings.PinLength); //Convert the Settings.PinLength to an Int
            DispatcherTimerSetup(); //Display the current date/time in a readable format and create the ticking "timer" to update the date/time every second

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

        private async void button_QuickPeek_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var paramDictionary = new Dictionary<string, string>();
                paramDictionary.Add("action", "EmployeeList");
                var wsCall = new WebServiceCall(Settings.ReadSetting("UriPrefix"), paramDictionary);

                //Retrieve the array of employees as a Json String
                //Expected Result: JSON Object
                HttpResponseMessage httpResponse = await wsCall.POST_JsonToWebApi();

                //Convert the Json string to a List of Employee Objects
                //TODO: Error Handling
                List<Employee> employeeList = JsonConvert.DeserializeObject<List<Employee>>(await httpResponse.Content.ReadAsStringAsync());

                //Stop the timer (not needed for the following page)
                DispatcherTimer.Stop();

                //Navigate to the next "QuickView" page and pass the employeeList object
                Frame.Navigate(typeof(QuickView), employeeList);
            }
            catch (Exception ex)
            { textBlock_Result.Text = ex.Message; }
        }

        //Config Button (Temporary)
        private void button_Config_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(Configuration), null); }
        
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

        private async void TryLogin()
        {
            try
            {
                var employee = new Employee();
                var paramDictionary = new Dictionary<string, string>();
                paramDictionary.Add("action", "PinLogin");
                paramDictionary.Add("pin", CurrentPin);
                var wsCall = new WebServiceCall(Settings.ValidateSetting("UriPrefix"), paramDictionary);
                HttpResponseMessage httpResponse = await wsCall.POST_JsonToWebApi();

                //Validation to ensure an employee object can be populated with accurate data

                //If a PIN is not in the database, the HttpResponse will return an empty array
                if ("[]" != await httpResponse.Content.ReadAsStringAsync())
                { employee = JsonConvert.DeserializeObject<Employee>(await httpResponse.Content.ReadAsStringAsync()); }
                else
                { employee = null; }

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
            catch (HttpRequestException)
            { textBlock_Result.Text = "Cannot connect to Apache server"; }
            catch (JsonException) //TODO: Do some proper error handling here, it's a lazy fix.  The error is because JsonConvert is trying to parse the string "Cannot connect to database", returned from the webservice when it cannot connect to the db
            { textBlock_Result.Text = "Something went wrong... Check your MySql connection"; }
        }
    }
}
