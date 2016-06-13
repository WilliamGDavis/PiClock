using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    public class Employee
    {
        public string id { get; set; }
        public string fname { get; set; }
        public string mname { get; set; }
        public string lname { get; set; }
        public string active { get; set; }
        public Job CurrentJob { get; set; }
        public List<Job> JobList { get; set; }

        //Check the database to see if a user is punched in currently
        public static async Task<HttpResponseMessage> CheckPunchedInStatus(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "CheckLoginStatus" },
                { "employeeId", employeeId }
            };
            return await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
        }

        //Return a list of all employees in the database
        public static async Task<HttpResponseMessage> TryGetEmployeeList()
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetEmployeeList" }
            };
            return await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
        }
    }
}
