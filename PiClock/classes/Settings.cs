﻿using System;
using System.Collections.Generic;
using Windows.Storage;

namespace PiClock.classes
{
    static class Settings
    {
        public static string PinLength { get; set; }
        public static string ApiServerAddress { get; set; }
        public static string ApiServerPort { get; set; }
        public static string ApiDirectory { get; set; }
        public static string UriPrefix { get; set; }
        public static string UseSsl { get; set; }
        public static Dictionary<string, string> ParamDictionary { get; set; } //Dictionary of all the application settings, set application-side
        private readonly static ApplicationDataContainer SettingsLocation = ApplicationData.Current.LocalSettings; //Use the application's "LocalSettings" folder to hold the settings values

        //Write the properties of this class to a settings file
        public static void WriteAllSettings()
        {
            if (null != ParamDictionary)
            {
                foreach (var row in ParamDictionary)
                { SettingsLocation.Values[row.Key] = row.Value; }
            }
        }

        //Read all the setting and pass them to this instance
        //If the setting does not exist, it will set the field to a null value
        public static void ReadAllSettings()
        {
            PinLength = ValidateSetting("PinLength");
            ApiServerAddress = ValidateSetting("ApiServerAddress");
            ApiServerPort = ValidateSetting("ApiServerPort");
            ApiDirectory = ValidateSetting("ApiDirectory");
            UriPrefix = ValidateSetting("UriPrefix");
            UseSsl = ValidateSetting("UseSsl");
        }

        //Write an individial value to settings
        //TODO: Validation
        public static void WriteSetting(string key, string value)
        { SettingsLocation.Values[key] = value; }

        //Return an individual setting's value
        //Expected result: string value of a setting, or a null if the setting doesn't exist
        public static string ReadSetting(string key)
        { return ValidateSetting(key); }

        //Used to "Clear" out the settings file, and all of it's values.  USE CAREFULLY!
        //This will completely blank the file out
        public async static void EraseAllSettings()
        { await ApplicationData.Current.ClearAsync(ApplicationDataLocality.Local); }

        //Read from the settings file and set the class fields
        //If the setting does not exist, set the field to a null value
        public static string ValidateSetting(string setting)
        {
            return setting =
                (null != SettingsLocation.Values[setting]) ?
                (string)SettingsLocation.Values[setting] :
                null;
        }

        //Try to parse the string value of PinLength to an int.  If it can't, return a 0 value
        public static int ConvertStringToInt(string settingValue)
        {
            int value = (Int32.TryParse(settingValue, out value)) ?
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
