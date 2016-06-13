using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace PiClock.classes
{
    public class AppSettings
    {
        //Valid Application Settings Values
        public static string PinLength { get; set; }
        public static string ApiServerAddress { get; set; }
        public static string ApiServerPort { get; set; }
        public static string ApiDirectory { get; set; }
        public static string ApiUsername { get; set; }
        public static string ApiPassword { get; set; }
        public static string UriPrefix { get; set; }
        public static string UseSsl { get; set; }
        public static string AllowPunchIntoJobWhenPunchingIn { get; set; }
    }

    static class Settings
    {
        public static Dictionary<string, string> ParamDictionary { get; set; } //Dictionary of all the application settings, set application-side
        private readonly static ApplicationDataContainer SettingsLocation = ApplicationData.Current.LocalSettings; //Use the application's "LocalSettings" folder to hold the settings values

        //Write an individial value to settings
        //TODO: Validation
        public static void Write(string key, string value)
        { SettingsLocation.Values[key] = value; }

        //Return an individual setting's value
        //Expected result: string value of a setting, or a null if the setting doesn't exist
        public static string Read(string key)
        { return ValidateSetting(key); }


        //Write the properties of this class to a settings file
        public static void WriteAllSettings()
        {
            if (null != ParamDictionary)
            {
                foreach (var row in ParamDictionary)
                { SettingsLocation.Values[row.Key] = row.Value; }
            }
        }



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
            int value = (int.TryParse(settingValue, out value)) ? value : 0;
            return value;
        }
    }

    public class DbSettings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    static class SettingsFromDB
    {

        public async static Task<string> GetSettingsFromDb(string uriPrefix, Dictionary<string, string> paramDictionary)
        {
            if (null != uriPrefix && null != paramDictionary)
            {
                string[] requiredParams = { "action", "ApiUsername", "ApiPassword" };
                var httpResponse = await CallWebService(uriPrefix, paramDictionary, requiredParams);
                return await httpResponse.Content.ReadAsStringAsync();
            }
            else
            { return null; }
        }

        private async static Task<HttpResponseMessage> CallWebService(string uriPrefix, Dictionary<string, string> paramDictionary, string[] requiredParams = null)
        {
            if (true == CommonMethods.CheckForRequiredParams(requiredParams, paramDictionary))
            {
                var wsCall = new WebServiceCall(paramDictionary);
                return await wsCall.PostJsonToRpcServer();
            }
            else
            { return null; }
        }
    }
}
