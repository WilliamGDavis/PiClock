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
        public Dictionary<string, string> ParamDictionary { get; set; }

        //Check the database to see if a user is logged in currently
        public static async Task<bool> TryCheckLoginStatus(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "CheckLoginStatus" },
                { "employeeId", employeeId }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            var httpResponse = await wsCall.PostJsonToRpcServer();

            //Should return true or false (will also return false if there was an error)
            var result = JsonConvert.DeserializeObject<string>(await httpResponse.Content.ReadAsStringAsync());
            return ("true" == result) ? true : false;
        }

        //Return a list of all employees in the database
        public static async Task<HttpResponseMessage> TryGetEmployeeList()
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetEmployeeList" }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            return await wsCall.PostJsonToRpcServer();
        }
    }
}
