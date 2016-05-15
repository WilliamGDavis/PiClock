//Copyright William G Davis 2016
using PiClock.classes;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Newtonsoft.Json;
using System.Net.Http;

namespace PiClock
{
    public sealed partial class MainPage : Page
    {
        //Settings File Values
        private int PinLength;
        private string UriPrefix;

        //Unconfigurable Properties
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();
            this.PinLength = 0;
            this.UriPrefix = null;

            ReadSettings(); //Read the Settings.xml file and assign the current values
            DispatcherTimerSetup(); //Display the current date/time in a readable format and create the ticking "timer" to update the date/time every second

            //Settings.EraseAllSettings();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        { textBlock_CurrentTime.Text = Format_dt_Current(DateTime.Now); }

        //Keypad Events
        private void btn_0_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_0.Content; }
        private void btn_1_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_1.Content; }
        private void btn_2_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_2.Content; }
        private void btn_3_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_3.Content; }
        private void btn_4_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_4.Content; }
        private void btn_5_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_5.Content; }
        private void btn_6_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_6.Content; }
        private void btn_7_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_7.Content; }
        private void btn_8_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_8.Content; }
        private void btn_9_Click(object sender, RoutedEventArgs e)
        { this.passwordBox_PIN.Password += this.btn_9.Content; }

        private async void button_QuickPeek_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Retrieve JSON from the WebService
                WebServiceCall webCall = new WebServiceCall();
                Settings setting = new Settings();
                Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
                ParamDictionary.Add("action", "get_all_employees");
                webCall.ParamDictionary = ParamDictionary;
                webCall.Uri = setting.ReadSetting("UriPrefix"); ;

                //Retrieve the array of employees as a Json String
                //Expected Result: JSON string
                HttpResponseMessage httpResponse = await webCall.POST_JsonToWebApi();
                string JsonString = await httpResponse.Content.ReadAsStringAsync();

                //Convert the Json string to a List of Employee Objects
                List<Employee> employeeList = JsonConvert.DeserializeObject<List<Employee>>(JsonString);

                //Stop the timer (not needed for the following page)
                this.dispatcherTimer.Stop();

                //Navigate to the next "QuickView" page and pass the employeeList object
                this.Frame.Navigate(typeof(QuickView), employeeList);
            }
            catch (Exception ex)
            { textBlock_Result.Text = ex.Message; }
        }

        //Config Button (Temporary)
        private void button_Config_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(Configuration), null); }

        private void passwordBox_PIN_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //TODO: Maybe consider checking validation first, and if all validation passes login.
            //Right now, it does all that in a single method
            try
            { Check_PIN(); }
            catch (Exception ex) //Never seems to get here
            { textBlock_Result.Text = ex.Message; }
        }

        public void DispatcherTimerSetup()
        {
            //Create a new timer and assign the Tick event
            //dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Tick += dispatcherTimer_Tick;

            //Set how often the tick will happen
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            //Start the Timer Function
            this.dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        { textBlock_CurrentTime.Text = Format_dt_Current(DateTime.Now); }

        private static string Format_dt_Current(DateTime dt)
        {
            //Format the 'Main' current date/time into a friendly structure
            string newString = dt.ToString("dddd MMMM dd, yyyy");
            newString += "\n";
            newString += dt.ToString("hh:mm:sstt");
            return newString;
        }

        private async void Check_PIN()
        {
            string PIN = passwordBox_PIN.Password;

            //Check to ensure the PIN length matches the configuration and is not 0 value
            if (PinLength != PIN.Length && 0!= PinLength)
            {
                if (PinLength <= PIN.Length)
                { passwordBox_PIN.Password = ""; }
                return;
            }
            else
            {
                try
                {
                    Employee employee = new Employee();
                    WebServiceCall wsCall = new WebServiceCall();
                    Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
                    ParamDictionary.Add("action", "Pin_Login");
                    ParamDictionary.Add("pin", PIN);
                    wsCall.ParamDictionary = ParamDictionary;
                    wsCall.Uri = UriPrefix;

                    HttpResponseMessage httpResponse = await wsCall.POST_JsonToWebApi();

                    //Validation to ensure an employee object can be populated with accurate data
                    if ("null" != await httpResponse.Content.ReadAsStringAsync())
                    { employee = JsonConvert.DeserializeObject<Employee>(await httpResponse.Content.ReadAsStringAsync()); }
                    else
                    { employee = null; }

                    if (null != employee)
                    { //Successful
                        passwordBox_PIN.Password = "";
                        Frame.Navigate(typeof(LoggedIn), employee);
                    }
                    else
                    { //Unsuccessful
                        textBlock_Result.Text = "Incorrect Login";
                        passwordBox_PIN.Password = "";
                        return;
                    }
                }
                catch (Exception ex)
                { return; }
            }
        }

        //Populate the class properties from the local settings file
        private void ReadSettings()
        {
            Settings settings = new Settings();
            settings.ReadAllSettings();

            PinLength = settings.ConvertPinLengthToInt(settings.PinLength);
            UriPrefix = settings.UriPrefix;
        }
        

    }
}
