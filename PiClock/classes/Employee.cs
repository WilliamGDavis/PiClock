using Newtonsoft.Json;
using System;
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
        [JsonProperty("EmployeeValues")]
        public EmployeeValues employeeValues;

        public async Task<Employee> TryLogin(string PIN, string Uri, string function)
        {
            Employee employee = await EmployeeDB.Login(PIN, Uri, function);
            if (null != employee)
            {
                //Set the Employee Object's Properties
                this.id = employee.id;
                this.fname = employee.fname;
                this.mname = employee.mname;
                this.lname = employee.lname;

                //Return the Employee object
                return this;
            }
            else
            { return null; }

        }

        private class EmployeeDB
        {
            public async static Task<Employee> Login(string PIN, string Uri, string function)
            { 
                //Connect to a web service and retrieve the data (JSON format)
                var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri(Uri));
                var JsonString = await response.Content.ReadAsStringAsync();

                //Determine the function to used based on the string passed in (May or may not be helpful)
                Employee model;
                switch (function)
                {
                    case "login_PIN":
                        model = JsonConvert.DeserializeObject<Employee>(JsonString);
                        break;
                    default: //Return an empty string
                        model = null;
                        break;
                }
                response.Dispose();
                return model;
            }
        }
    }

    public class EmployeeValues : Employee
    {
        public bool loggedIn;
    }
}
