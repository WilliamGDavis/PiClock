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

        public static async Task<HttpResponseMessage> GetCurrentJob(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetCurrentJob" },
                { "employeeId", employeeId }
            };
            return await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
        }
    }
}
