//Copyright William G Davis 2016
using PiClock.classes;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
                if (Settings.ConvertStringToInt(Settings.Read("PinLength")) == _currentPin.Length)
                { Login(); }
            }
        }

        public MainPage()
        {
            InitializeComponent();
            DispatcherTimerSetup(); //Display the current date/time in a readable format and create the ticking "timer" to update the date/time every second
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        { textBlock_CurrentTime.Text = Format_dt_Current(DateTime.Now); }

        #region KeyPad Buttons
        //Keypad functionality (Structured around Touch-based user input)
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
        #endregion

        //Get the list of employees from the database and navigate to the "QuickView" page
        private async void button_QuickPeek_Click(object sender, RoutedEventArgs e)
        {
            var employeeList = await Employee.GetEmployeeList();

            //Stop the timer (not needed for the following page)
            DispatcherTimer.Stop();

            //Navigate to the next "QuickView" page and pass the employeeList object
            Frame.Navigate(typeof(QuickView), employeeList);
        }

        //Build a new DispatcherTimer to display the current datetime
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

        //Update the current datetime
        void DispatcherTimer_Tick(object sender, object e)
        { textBlock_CurrentTime.Text = Format_dt_Current(DateTime.Now); }

        //Format the 'Main' current date/time into a user-friendly structure
        //  Monday, June 20 2016
        //      10:54:25AM
        private static string Format_dt_Current(DateTime dt)
        { return dt.ToString("dddd MMMM dd, yyyy\nhh:mm:sstt"); }

        //Login to an employee's page using a PIN
        private async void Login()
        {
            var employee = await Authentication.TryLogin(CurrentPin);

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
    }
}
