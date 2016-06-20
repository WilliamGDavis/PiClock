using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class Authentication
    {
        /**
        <summary>
            Attempt to login to the database using a PIN
        </summary>
        <returns>
            Employee object (if successful)
            null (if unsuccessful)
        </returns>
        */
        public static async Task<Employee> TryLogin(string pin)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PinLogin" },
                { "pin", pin }
            };

            var httpResponse = await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
            var httpContent = await httpResponse.Content.ReadAsStringAsync();
            var employee = (Employee)CommonMethods.Deserialize(typeof(Employee), httpContent);
            return employee;
        }
    }
}
