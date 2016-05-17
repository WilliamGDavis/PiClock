using System;
using System.Collections.Generic;
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

        //Read all the setting and pass them to this instance
        //If the setting does not exist, it will set the field to a null value
        public void ReadAllSettings()
        {
            PinLength = ValidateSetting("PinLength");
            ApiServerAddress = ValidateSetting("ApiServerAddress");
            ApiServerPort = ValidateSetting("ApiServerPort");
            ApiDirectory = ValidateSetting("ApiDirectory");
            UriPrefix = ValidateSetting("UriPrefix");
        }

        //Write an individial value to settings
        //TODO: Validation
        public void WriteSetting(string key, string value)
        { SettingsLocation.Values[key] = value; }

        //Return an individual setting's value
        //Expected result: string value of a setting, or a null if the setting doesn't exist
        public string ReadSetting(string key)
        { return ValidateSetting(key); }

        //Used to "Clear" out the settings file, and all of it's values.  USE CAREFULLY!
        //This will completely blank the file out
        public async static void EraseAllSettings()
        { await ApplicationData.Current.ClearAsync(ApplicationDataLocality.Local); }

        //Read from the settings file and set the class fields
        //If the setting does not exist, set the field to a null value
        public string ValidateSetting(string setting)
        {
            return setting =
                (null != SettingsLocation.Values[setting]) ?
                (string)SettingsLocation.Values[setting] :
                null;
        }

        //Try to parse the string value of PinLength to an int.  If it can't, return a 0 value
        public int ConvertPinLengthToInt(string settingsPinLength)
        {
            int value = (Int32.TryParse(settingsPinLength, out value)) ?
                    value :
                    0;
            return value;
        }
    }

    class SettingsFromDB
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
