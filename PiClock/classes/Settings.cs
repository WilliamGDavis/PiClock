using System;
using Windows.Storage;

namespace PiClock.classes
{
    class Settings
    {
        public string PinLength { get; set; }
        public string ApiServerAddress { get; set; }
        public string ApiServerPort { get; set; }
        public string ApiDirectory { get; set; }
        public string UriPrefix { get; set; }
        private ApplicationDataContainer SettingsLocation = ApplicationData.Current.LocalSettings;

        public Settings()
        {
            PinLength = null;
            ApiServerAddress = null;
            ApiServerPort = null;
            ApiDirectory = null;
            UriPrefix = null;
        }

        //Write the properties of this instance to the setting file
        //TODO: Validation
        public void WriteAllSettings()
        {
            SettingsLocation.Values["PinLength"] = PinLength;
            SettingsLocation.Values["ApiServerAddress"] = ApiServerAddress;
            SettingsLocation.Values["ApiServerPort"] = ApiServerPort;
            SettingsLocation.Values["ApiDirectory"] = ApiDirectory;
            SettingsLocation.Values["UriPrefix"] = UriPrefix;
        }

        //Write an individial value to settings
        //TODO: Validation
        public void WriteSetting(string key, string value)
        { SettingsLocation.Values[key] = value; }

        //Read all the setting and pass them to this instance
        public void ReadAllSettings()
        {
            PinLength = (string)SettingsLocation.Values["PinLength"];
            ApiServerAddress = (string)SettingsLocation.Values["ApiServerAddress"];
            ApiServerPort = (string)SettingsLocation.Values["ApiServerPort"];
            ApiDirectory = (string)SettingsLocation.Values["ApiDirectory"];
            UriPrefix = (string)SettingsLocation.Values["UriPrefix"];
        }

        //Return an individual setting's value
        //Expected result: string
        public string ReadSetting(string key)
        { return (string)SettingsLocation.Values[key]; }

        //Used to "Clear" out the settings file, and all of it's values.  USE CAREFULLY!
        //This will completely blank the file out
        public async static void EraseAllSettings()
        { await ApplicationData.Current.ClearAsync(ApplicationDataLocality.Local); }
    }
}
