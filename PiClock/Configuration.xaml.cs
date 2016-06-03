using Newtonsoft.Json;
using PiClock.classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PiClock
{
    public sealed partial class Configuration : Page
    {
        public Configuration()
        { InitializeComponent(); }

        //Read from local settings and populate the appropriate fields with their values
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            //Read from local settings and display them to the user
            Settings.ReadLocalSettings();

            //Validate for null settings values and fill in the appropriate textboxes
            textBox_ApiServerAddress.Text = CommonMethods.ConvertNullStringToEmptyString(Settings.ApiServerAddress);
            textBox_ApiServerPort.Text = CommonMethods.ConvertNullStringToEmptyString(Settings.ApiServerPort);
            textBox_ApiDirectory.Text = CommonMethods.ConvertNullStringToEmptyString(Settings.ApiDirectory);
            if ("s" == CommonMethods.ConvertNullStringToEmptyString(Settings.UseSsl))
            { checkBox_UseSsl.IsChecked = true; }
            if ("true" == CommonMethods.ConvertNullStringToEmptyString(Settings.AllowPunchIntoJobWhenPunchingIn))
            { checkBox_AllowJobPunchWhenPunchingIn.IsChecked = true; }

            IsEnabled = true;
        }

        //Cancel Button Event
        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(Launcher), null); }

        //Save Changes Button Event
        private async void button_SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;

            //Used as a "loading" placeholder
            textBlock_AllSettings.Text = "Saving settings..."; 

            //Check for a valid database connection before writing to local settings
            if (!true == await TryCheckDbConnection())
            {
                IsEnabled = true;
                textBlock_AllSettings.Text = "An Error Occured: Check database settings";
                return;
            }

            //Retrieve the settings from the database in a JSON string
            //If the deserialization fails, populate an empty dbSettings object
            var dbSettings = JsonConvert.DeserializeObject<List<DbSettings>>(await TryGetSettingsFromDb(), new JsonSerializerSettings { Error = CommonMethods.HandleDeserializationError });

            //Erase all previous settings and write the new settings
            Settings.EraseAllSettings();
            WriteSettings(dbSettings);

            IsEnabled = true;
            Frame.Navigate(typeof(Launcher), null);
        }

        //Test Connection Button Event
        private async void button_TestConnection_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            textBlock_AllSettings.Text = "";

            //Used as a "Loading" placeholder
            textBlock_ConnectionStatus.Text = "Checking Connection...";

            //Check for an valid connection to the database
            if (!true == await TryCheckDbConnection())
            {
                IsEnabled = true;
                textBlock_ConnectionStatus.Text = "Unsuccessful";
                return;
            }

            IsEnabled = true;
            textBlock_ConnectionStatus.Text = "Successful!";
        }

        //Update Settings Button Event
        //Pulls the settings down from the database
        private async void button_ViewDbSettings_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            textBlock_ConnectionStatus.Text = "";

            //Used as a "loading" placeholder
            textBlock_AllSettings.Text = "Retrieving Settings from Database...";

            //Check for a valid connection to the database
            if (!true == await TryCheckDbConnection())
            {
                IsEnabled = true;
                textBlock_AllSettings.Text = "An Error Occured: Check database settings";
                return;
            }

            //Retrieve the settings from the database in a JSON string and parse them into a list of DbSettings
            var dbSettings = JsonConvert.DeserializeObject<List<DbSettings>>(await TryGetSettingsFromDb(), new JsonSerializerSettings { Error = CommonMethods.HandleDeserializationError });

            //Display the settings to the user
            textBlock_AllSettings.Text = BuildSettingsList(dbSettings);
            IsEnabled = true;
        }

        //Check for a vaild connection to the web service and database
        private async Task<bool> TryCheckDbConnection()
        {
            //TODO: Check for valid values in the textboxes
            string uriPrefix = String.Format("http://{0}{1}:{2}{3}", CommonMethods.ValidateSimpleString(textBox_ApiServerAddress.Text, 1, 255, true),
                                                                     (true == checkBox_UseSsl.IsChecked) ? "s" : "",
                                                                     CommonMethods.ValidateSimpleString(textBox_ApiServerPort.Text, 1, 5, true),
                                                                     CommonMethods.ValidateSimpleString(textBox_ApiDirectory.Text, 0, 255, true));

            try
            {
                DbFunctions dbConn = new DbFunctions();
                dbConn.Uri = String.Format("{0}action=test_connection", uriPrefix);


                //Check the db connection.  
                //Expected result: "true" or "false"
                return ("true" == await dbConn.CheckDBConnection()) ? true : false;
            }
            catch (Exception ex)
            { return false; }
        }

        //Attempt to pull the database settings
        private async Task<string> TryGetSettingsFromDb()
        {
            try
            {
                var paramDictionary = new Dictionary<string, string>();
                paramDictionary.Add("action", "GetSettings");
                Settings.ParamDictionary = paramDictionary;
                Settings.UriPrefix = String.Format("http://{0}{1}:{2}{3}", CommonMethods.ValidateSimpleString(textBox_ApiServerAddress.Text, 1, 255, true),
                                                                           (true == checkBox_UseSsl.IsChecked) ? "s" : "",
                                                                           CommonMethods.ValidateSimpleString(textBox_ApiServerPort.Text, 1, 255, true),
                                                                           CommonMethods.ValidateSimpleString(textBox_ApiDirectory.Text, 0, 255, true));
                return await SettingsFromDB.GetSettingsFromDb();
            }
            catch (Exception ex)
            { return ex.Message; }
        }

        //Write settings to local settings
        private void WriteSettings(List<DbSettings> dbSettings = null)
        {
            string apiServerAddress = CommonMethods.ValidateSimpleString(textBox_ApiServerAddress.Text, 1, 255, true);
            string apiServerPort = CommonMethods.ValidateSimpleString(textBox_ApiServerPort.Text, 1, 5, true);
            string apiDirectory = CommonMethods.ValidateSimpleString(textBox_ApiDirectory.Text, 0, 255, true);
            string useSsl = (true == checkBox_UseSsl.IsChecked) ? "s" : "";
            string allowPunchIntoJobWhenPunchingIn = (true == checkBox_AllowJobPunchWhenPunchingIn.IsChecked) ? "true" : "false";
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("ApiServerAddress", apiServerAddress);
            paramDictionary.Add("ApiServerPort", apiServerPort);
            paramDictionary.Add("ApiDirectory", apiDirectory);
            paramDictionary.Add("UseSsl", useSsl);
            paramDictionary.Add("AllowPunchIntoJobWhenPunchingIn", allowPunchIntoJobWhenPunchingIn); 
            paramDictionary.Add("UriPrefix", String.Format("http{0}://{1}:{2}/{3}", useSsl,
                                                                                    apiServerAddress,
                                                                                    apiServerPort,
                                                                                    apiDirectory));

            //Write settings collected from the database to the application settings file
            if (null != dbSettings)
            {
                foreach (var setting in dbSettings)
                { paramDictionary.Add(setting.Name, setting.Value); }
            }

            //Pass the param dictionary to the Settings static class
            Settings.ParamDictionary = paramDictionary;

            //Write the values to the settings location
            Settings.WriteAllSettings();
        }

        //Format the setting into a human-readable string
        private string BuildSettingsList(List<DbSettings> settingsList = null)
        {
            var settingsString = new StringBuilder();
            settingsString.Append("Settings:\n");
            settingsString.Append(Environment.NewLine);
            foreach (var setting in settingsList)
            {
                settingsString.Append(setting.Name);
                settingsString.Append(": ");
                settingsString.Append(setting.Value);
                settingsString.Append(Environment.NewLine);
            }
            return settingsString.ToString();
        }
    }
}
