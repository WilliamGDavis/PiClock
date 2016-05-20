using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class Punch
    {
        public Employee Employee { get; set; }
        public Dictionary<string, string> ParamDictionary { get; set; }

        public async Task<string> PunchIn()
        {
            string[] requiredParams = { "action", "employeeId" };
            return await CallWebService(requiredParams);
        }

        public async Task<string> PunchOut()
        {
            string[] requiredParams = { "action", "employeeId", "currentJobId" };
            return await CallWebService(requiredParams);
        }

        public async Task<string> PunchIntoJob()
        {
            string[] requiredParams = { "action", "employeeId", "currentJobId", "newJobId" };
            return await CallWebService(requiredParams);
        }


        private async Task<string> CallWebService(string[] requiredParams = null)
        {
            if (true == CommonMethods.CheckForRequiredParams(requiredParams, ParamDictionary) &&
                null != Employee
                )
            { return await CommonMethods.ReturnStringFromWebService(ParamDictionary); }
            else
            { return null; }
        }
    }
}
