using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class Punch
    {
        public async Task<string> PunchIn(Employee employee)
        {
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "PunchIn");
            paramDictionary.Add("employeeId", employee.id.ToString());
            var wsCall = new WebServiceCall(Settings.ValidateSetting("UriPrefix"), paramDictionary);
            var httpResponse = new HttpResponseMessage();
            httpResponse = await wsCall.POST_JsonToWebApi();
            return await httpResponse.Content.ReadAsStringAsync();
        }

        public async Task<string> PunchOut(Employee employee)
        {
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "PunchOut");
            paramDictionary.Add("employeeId", employee.id.ToString());
            paramDictionary.Add("currentJobId", (null != employee.CurrentJob) ? employee.CurrentJob.Id : "null"); //If a user is not currently logged into a job, set a null string value
            var wsCall = new WebServiceCall(Settings.ValidateSetting("UriPrefix"), paramDictionary);
            HttpResponseMessage httpResponse = await wsCall.POST_JsonToWebApi();
            return await httpResponse.Content.ReadAsStringAsync();
        }

        public async Task<string> PunchIntoJob(Employee employee, string newJobId)
        {
            var paramDictionary = new Dictionary<string, string>();
            paramDictionary.Add("action", "PunchIntoJob");
            paramDictionary.Add("employeeId", employee.id.ToString());
            paramDictionary.Add("currentJobId", (null != employee.CurrentJob) ? employee.CurrentJob.Id : "null"); //If a user is not currently logged into a job, set a null string value
            paramDictionary.Add("newJobId", newJobId);
            var wsCall = new WebServiceCall(Settings.ValidateSetting("UriPrefix"), paramDictionary);
            HttpResponseMessage httpResponse = await wsCall.POST_JsonToWebApi();
            return await httpResponse.Content.ReadAsStringAsync();
        }

    }
}
