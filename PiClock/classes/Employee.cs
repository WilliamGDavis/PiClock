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

        /**
        <summary>
            Return a list of all employess from the database
        </summary>
        <returns>
            List<Employee> (if successful)
            null (if unsuccessful)
        </returns>
        */
        public static async Task<List<Employee>> GetEmployeeList()
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetEmployeeList" }
            };

            var httpResponse = await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
            var httpContent = await httpResponse.Content.ReadAsStringAsync();
            var employeeList = (List<Employee>)CommonMethods.Deserialize(typeof(List<Employee>), httpContent);
            return employeeList;
        }
    }
}
