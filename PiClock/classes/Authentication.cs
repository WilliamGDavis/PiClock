using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class Authentication
    {
        public Dictionary<string, string> ParamDictionary { get; set; }

        public async Task<string> Login()
        {
            string[] requiredParams = { "action", "pin" };
            return await CallWebService(requiredParams);
        }

        private async Task<string> CallWebService(string[] requiredParams = null)
        {
            if (true == CommonMethods.CheckForRequiredParams(requiredParams, ParamDictionary))
            { return await CommonMethods.ReturnStringFromWebService(ParamDictionary); }
            else
            { return null; }
        }
    }
}
