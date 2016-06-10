using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    public class Punch
    {
        //Lookup a jobId in the database based on the jobDescription
        public static async Task<string> JobLookup(string jobDescription)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                {"action", "GetJobIdByJobDescription" },
                {"jobDescription", jobDescription }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            var httpResponse = await wsCall.PostJsonToRpcServer();
            return await httpResponse.Content.ReadAsStringAsync();
        }

        //Attempt to punch into a new job while punching out of an old job, if necessary
        public static async Task PunchIntoNewJob(Employee employee, string newJobId)
        {
            string currentJobId = (null != employee.CurrentJob) ? employee.CurrentJob.Id.ToString() : "null";
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PunchIntoJob" },
                { "employeeId", employee.id },
                { "currentJobId",  currentJobId },
                { "newJobId", newJobId }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            await wsCall.PostJsonToRpcServer();
        }

        //Punch in to the database (Regular Punch)
        public static async Task<bool> PunchIn(string employeeId)
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PunchIn" },
                { "employeeId", employeeId }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            var httpResponse = await wsCall.PostJsonToRpcServer();

            //TODO: Verify the returned data from the RPC server and make sure that this is even necessary
            return (null != httpResponse) ? true : false;
        }

        //Punch out of the database
        public static async Task<bool> PunchOut(string employeeId, string currentJobId)
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PunchOut" },
                { "employeeId", employeeId },
                { "currentJobId", currentJobId }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            var httpResponse = await wsCall.PostJsonToRpcServer();

            //TODO: Verify the returned data from the RPC server and make sure that this is even necessary
            if (null != httpResponse)
            { return true; }
            else
            { return false; }
        }

        //Return an array of the punches for the day
        public static async Task<HttpResponseMessage> GetTodaysPunches(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetSingleDayPunchesByEmployeeId" },
                { "employeeId", employeeId }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            return await wsCall.PostJsonToRpcServer();
        }

        //Return an array of the punches for the week
        public static async Task<HttpResponseMessage> TryGetThisWeeksPunches(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetThisWeeksPunchesByEmployeeId" },
                { "employeeId", employeeId }
            };
            var wsCall = new WebServiceCall(paramDictionary);
            return await wsCall.PostJsonToRpcServer();
        }
    }

    class EmployeePunchesByDay
    {
        public RegularPunchOpen RegularPunchesOpen { get; set; }
        public JobPunchOpen JobPunchesOpen { get; set; }
        public RegularPunchesPaired RegularPunchesPaired { get; set; }
        public JobPunchesPaired JobPunchesPaired { get; set; }
        
    }

    class EmployeePunchesByWeek
    {
        public List<PairedPunches> PairedPunches { get; set; }
        public OpenPunches OpenPunches { get; set; }
    }

    class PairedPunches
    {
        public DateTime? Date { get; set; }
        public string DayName { get; set; }
        public List<RegularPunchesPaired> RegularPunchesPaired { get; set; }
        public List<JobPunchesPaired> JobPunchesPaired { get; set; }
    }

    class OpenPunches
    {
        public RegularPunchOpen RegularPunchesOpen { get; set; }
        public JobPunchOpen JobPunchesOpen { get; set; }
    }

    class WeekdayPunch
    {
        public DateTime? Date { get; set; }
        public string DayName { get; set; }
        public List<RegularPunchesPaired> RegularPunchesPaired { get; set; }
        public List<JobPunchesPaired> JobPunchesPaired { get; set; }
        public List<RegularPunchOpen> RegularPunchesOpen { get; set; }
        public List<JobPunchOpen> JobPunchesOpen { get; set; }
    }

    class PairedPunch
    {
        public string ParentId { get; set; }
        public string ChildId { get; set; }
        public DateTime? PunchIn { get; set; }
        public DateTime? PunchOut { get; set; }
        public double? DurationInSeconds { get; set; }
    }

    class OpenPunch
    {
        public string OpenId { get; set; }
        public string ParentId { get; set; }
        public DateTime? PunchIn { get; set; }
    }

    class RegularPunchesPaired
    {
        public double? TotalDurationInSeconds { get; set; }
        public List<PairedPunch> Punches { get; set; }
    }

    class JobPunchesPaired
    {
        public double? TotalDurationInSeconds { get; set; }
        public List<JobPunch> Punches { get; set; }
    }

    class RegularPunch : PairedPunch { } //Not Used
    class RegularPunchOpen : OpenPunch { }

    class JobPunch : PairedPunch
    { public JobInformation JobInformation { get; set; } }

    class JobPunchOpen : OpenPunch
    { public JobInformation JobInformation { get; set; } }

    class JobInformation
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }


}
