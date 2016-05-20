using Newtonsoft.Json;
using PiClock.classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PiClock
{
    public sealed partial class Configuration : Page
    {

        public Configuration()
        { this.InitializeComponent(); }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Read from local settings and display them to the user
            //Settings settings = new Settings();
            Settings.ReadAllSettings();

            //Validate for null settings values and fill in the appropriate textboxes
            textBox_PinLength.Text = CheckForNullSetting(Settings.PinLength);
            textBox_ApiServerAddress.Text = CheckForNullSetting(Settings.ApiServerAddress);
            textBox_ApiServerPort.Text = CheckForNullSetting(Settings.ApiServerPort);
            textBox_ApiDirectory.Text = CheckForNullSetting(Settings.ApiDirectory);
            if ("s" == CheckForNullSetting(Settings.UseSsl))
            { checkBox_UseSsl.IsChecked = true; }
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(Launcher), null); }

        private void button_SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            //Write the settings to the local settings
            WriteSettings();
            Frame.Navigate(typeof(Launcher), null);
        }

        private async void button_TestConnection_Click(object sender, RoutedEventArgs e)
        {
            //Used as a "Loading" placeholder
            textBlock_ConnectionStatus.Text = "Checking Connection...";

            //TODO: Check for valid values in the textboxes
            string uriPrefix = String.Format("http://{0}{1}:{2}{3}", textBox_ApiServerAddress.Text,
                                                                     (true == checkBox_UseSsl.IsChecked) ? "s" : "",
                                                                     textBox_ApiServerPort.Text,
                                                                     textBox_ApiDirectory.Text);

            //Check for a vaild connection to the web service
            try
            {
                //create a new WebServiceCall : DbFunctions Object and pass in the Uri
                DbFunctions dbConn = new DbFunctions();
                dbConn.Uri = string.Format("{0}action=test_connection", uriPrefix);

                //Check the db connection.  
                //Expected result: "true" or "false"
                string connectionStatus = await dbConn.CheckDBConnection();

                //Display the result to the user
                if ("true" == connectionStatus)
                { textBlock_ConnectionStatus.Text = "Successful!"; }
                else
                { textBlock_ConnectionStatus.Text = "Unsuccessful"; }
            }
            catch (Exception ex)
            { textBlock_ConnectionStatus.Text = ex.Message; }
        }

        private void WriteSettings()
        {
            //TODO: Validate before passing text values to the local settings file
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("PinLength", textBox_PinLength.Text); //TODO: Write this to the database instead of the local application
            paramDictionary.Add("ApiServerAddress", textBox_ApiServerAddress.Text);
            paramDictionary.Add("ApiServerPort", textBox_ApiServerPort.Text);
            paramDictionary.Add("ApiDirectory", textBox_ApiDirectory.Text);
            paramDictionary.Add("UseSsl", (true == checkBox_UseSsl.IsChecked) ? "s" : "");
            paramDictionary.Add("UriPrefix", String.Format("http://{0}{1}:{2}{3}", Settings.ApiServerAddress, //TODO: Handle https if selected to use SSL
                                                                                   Settings.UseSsl,
                                                                                   Settings.ApiServerPort,
                                                                                   Settings.ApiDirectory));
            Settings.ParamDictionary = paramDictionary;

            //Write the values to the settings location
            Settings.WriteAllSettings();
        }

        //Check to ensure a setting value is not null, and if it is return an empty string
        private string CheckForNullSetting(string setting)
        { return setting = (null != setting) ? setting : ""; }

        private async void button_UpdateSettings_Click(object sender, RoutedEventArgs e)
        {
            textBlock_AllSettings.Text = "Updating Settings from Database...";
            //Settings settings = new Settings();
            Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            WebServiceCall wsCall = new WebServiceCall();
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            wsCall.Uri = Settings.ValidateSetting("UriPrefix");
            ParamDictionary.Add("action", "GetSettings");
            wsCall.ParamDictionary = ParamDictionary;

            httpResponse = await wsCall.POST_JsonToWebApi();
            textBlock_AllSettings.Text = await BuildSettingsList(httpResponse);

            //TODO: Write the settings to the local settings directory
        }

        //Ugly way to pull down all settings into a text box.
        //TODO: Match the updated setting against the setting stored locally
        private async Task<string> BuildSettingsList(HttpResponseMessage httpResponse)
        {
            StringBuilder settingsString = new StringBuilder();
            settingsString.Append("Settings:\n");
            settingsString.Append(Environment.NewLine);
            var settingsValues = JsonConvert.DeserializeObject<List<SettingsFromDB>>(await httpResponse.Content.ReadAsStringAsync());
            foreach (var setting in settingsValues)
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
