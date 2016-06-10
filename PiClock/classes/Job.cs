using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PiClock.classes
{
    public class Job
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Active { get; set; }
        public Employee Employee { get; set; }
        public Dictionary<string, string> ParamDictionary { get; set; }

        public async Task<string> GetCurrentJob()
        {
            string[] requiredParams = { "action", "employeeId" };
            if (true == CommonMethods.CheckForRequiredParams(requiredParams, ParamDictionary) &&
                null != ParamDictionary)
            {
                var wsCall = new WebServiceCall(ParamDictionary);
                HttpResponseMessage httpResponse = await wsCall.PostJsonToRpcServer();
                return await httpResponse.Content.ReadAsStringAsync();
            }
            else
            { return null; }
        }
    }
}
