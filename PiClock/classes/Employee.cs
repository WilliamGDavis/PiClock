using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    public class Employee
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("fname")]
        public string fname { get; set; }
        [JsonProperty("mname")]
        public string mname { get; set; }
        [JsonProperty("lname")]
        public string lname { get; set; }
        [JsonProperty("active")]
        public string active { get; set; }
        [JsonProperty("currentJob")]
        public Job CurrentJob { get; set; }
        [JsonProperty("jobList")]
        public List<Job> JobList { get; set; }

        public Dictionary<string, string> ParamDictionary { get; set; }
        
        public async Task<bool> CheckLoginStatus()
        {
            string[] requiredParams = { "action", "employeeId" };
            if ("true" == await CallWebService(requiredParams)) //this refers to the current instance of the class
            { return true; }
            else
            { return false; }
        }

        private async Task<string> CallWebService(string[] requiredParams = null)
        {
            if (true == CommonMethods.CheckForRequiredParams(requiredParams, ParamDictionary) &&
                null != this &&
                null != ParamDictionary
                )
            { return await CommonMethods.ReturnStringFromWebService(ParamDictionary); }
            else
            { return null; }
        }
    }
}
