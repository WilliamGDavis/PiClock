using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        ////Public-Facing method to call a webservice and return some string data (WebService will return JSON)
        ////Employee object is required (Probably a bad idea for some calls)
        //public static async Task<string> CallWebService(string[] requiredParams = null, Employee employee = null, Dictionary<string, string> paramDictionary = null)
        //{
        //    if (true == CheckForRequiredParams(requiredParams, paramDictionary) &&
        //        null != employee &&
        //        null != paramDictionary
        //        )
        //    { return await ReturnStringFromWebService(paramDictionary); }
        //    else
        //    { return null; }
        //}

        //Used to read a from a webservice that returns a JSON string
        public static async Task<string> ReturnStringFromWebService(Dictionary<string, string> paramDictionary = null)
        {
            var wsCall = new WebServiceCall(Settings.ValidateSetting("UriPrefix"), paramDictionary);
            HttpResponseMessage httpResponse = await wsCall.POST_JsonToWebApi();
            return await httpResponse.Content.ReadAsStringAsync();
        }
    }
}
