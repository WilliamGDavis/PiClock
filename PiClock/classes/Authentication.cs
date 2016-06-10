using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class Authentication
    {
        //Check the databse for an employee with the PIN passed in
        public static async Task<HttpResponseMessage> TryLogin(string pin)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PinLogin" },
                { "pin", pin }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            return await wsCall.PostJsonToRpcServer();
        }
    }
}
