using PiClock.classes;
using System;
using System.Collections.Generic;
using Windows.Storage;
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

            textBox_PinLength.Text = settings.PinLength;
            textBox_ApiServerAddress.Text = settings.ApiServerAddress;
            textBox_ApiServerPort.Text = settings.ApiServerPort;
            textBox_ApiDirectory.Text = settings.ApiDirectory;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        { Frame.Navigate(typeof(MainPage), null); }

        private void button_SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            //Write the settings to the local settings
            WriteSettings();
            Frame.Navigate(typeof(MainPage), null);
        }

        private async void button_TestConnection_Click(object sender, RoutedEventArgs e)
        {
            //Used as a "Loading" placeholder
            textBlock_ConnectionStatus.Text = "Checking Connection...";

            //TODO: Check for valid values in the textboxes
            string uriPrefix = String.Format("http://{0}:{1}{2}", textBox_ApiServerAddress.Text, textBox_ApiServerPort.Text, textBox_ApiDirectory.Text);

            //Check for a vaild connection to the web service
            try
            {
                //create a new WebServiceCall : DbFunctions Object and pass in the Uri
                DbFunctions dbConn = new DbFunctions();
                dbConn.Uri = string.Format("{0}action=test_connection", uriPrefix);

                //Check the connection.  
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
    }


}
