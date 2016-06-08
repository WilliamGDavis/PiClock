using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    public class Employee
    {
        public int id { get; set; }
        public string fname { get; set; }
        public string mname { get; set; }
        public string lname { get; set; }
        public string active { get; set; }
        public Job CurrentJob { get; set; }
        public List<Job> JobList { get; set; }
        public Dictionary<string, string> ParamDictionary { get; set; }
        
        public async Task<string> CheckLoginStatus()
        {
            string[] requiredParams = { "action", "employeeId" };
            return await CallWebService(requiredParams);
        }

        public async Task<string> GetEmployeeList()
        {
            string[] requiredParams = { "action" };
            return await CallWebService(requiredParams);
        }

        public async Task<string> CallWebService(string[] requiredParams = null)
        {
            if (true == CommonMethods.CheckForRequiredParams(requiredParams, ParamDictionary) &&
                null != this
                )
            { return await CommonMethods.GetJsonFromRpcServer(ParamDictionary); }
            else
            { return null; }
        }
    }
}
