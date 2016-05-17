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
            Settings settings = new Settings();
            settings.ReadAllSettings();

            //Validate for null settings values and fill in the appropriate textboxes
            textBox_PinLength.Text = CheckForNullSetting(settings.PinLength);
            textBox_ApiServerAddress.Text = CheckForNullSetting(settings.ApiServerAddress);
            textBox_ApiServerPort.Text = CheckForNullSetting(settings.ApiServerPort);
            textBox_ApiDirectory.Text = CheckForNullSetting(settings.ApiDirectory);
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
            string uriPrefix = String.Format("http://{0}:{1}{2}", textBox_ApiServerAddress.Text, 
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
            //TODO: Validate
            Settings settings = new Settings();
            settings.PinLength = textBox_PinLength.Text;
            settings.ApiServerAddress = textBox_ApiServerAddress.Text;
            settings.ApiServerPort = textBox_ApiServerPort.Text;
            settings.ApiDirectory = textBox_ApiDirectory.Text;
            settings.UriPrefix = String.Format("http://{0}:{1}{2}", settings.ApiServerAddress,
                                                                    settings.ApiServerPort,
                                                                    settings.ApiDirectory);

            //Write the values to the settings location
            settings.WriteAllSettings();
        }

        //Check to ensure a setting value is not null, and if it is return an empty string
        private string CheckForNullSetting(string setting)
        { return setting = (null != setting) ? setting : ""; }

        private async void button_UpdateSettings_Click(object sender, RoutedEventArgs e)
        {
            textBlock_AllSettings.Text = "Updating Settings from Database...";
            Settings settings = new Settings();
            Dictionary<string, string> ParamDictionary = new Dictionary<string, string>();
            WebServiceCall wsCall = new WebServiceCall();
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            wsCall.Uri = settings.ValidateSetting("UriPrefix");
            ParamDictionary.Add("action", "RetrieveSettings");
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
