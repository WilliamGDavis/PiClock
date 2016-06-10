using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiClock.classes
{
    static class CommonMethods
    {
        //Check to ensure the required parameters exist in the dictionary, and there are no additional paramaters in the dictionary
        public static bool CheckForRequiredParams(string[] requiredParams = null, Dictionary<string, string> paramDictionary = null)
        {
            foreach (var param in paramDictionary)
            {
                if (!requiredParams.Contains(param.Key))
                { return false; }
            }
            return true;
        }

        public static void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        //Used to read a from a webservice that returns a JSON string
        public static async Task<string> GetJsonFromRpcServer(Dictionary<string, string> paramDictionary)
        {
            try
            {
                var wsCall = new WebServiceCall(paramDictionary);
                HttpResponseMessage httpResponse = await wsCall.PostJsonToRpcServer();
                return await httpResponse.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            { return ex.Message; }
        }

        public static string ValidateSimpleString(string stringToValidate = null, int minLength = 0, int maxLength = 0, bool allowNumerals = false)
        {
            string regex = (true == allowNumerals) ? @"\w[a-zA-Z0-9_-]" : @"\w[a-zA-Z_-]";
            if (!Regex.IsMatch(stringToValidate, regex))
            { return null; }
            if (minLength > stringToValidate.Length || maxLength < stringToValidate.Length)
            { return null; }
            return stringToValidate;
        }

        //Check to ensure a setting value is not null, and if it is return an empty string
        public static string ConvertNullStringToEmptyString(string stringToConvert = null)
        { return stringToConvert = (null != stringToConvert) ? stringToConvert : ""; }
    }
}
