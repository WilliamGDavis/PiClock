using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class Punch
    {

        public Employee Employee { get; set; }
        public Dictionary<string, string> ParamDictionary { get; set; }

        public async Task<string> PunchIn()
        {
            string[] requiredParams = { "action", "employeeId" };
            return await CallWebService(requiredParams);
        }

        public async Task<string> PunchOut()
        {
            string[] requiredParams = { "action", "employeeId", "currentJobId" };
            return await CallWebService(requiredParams);
        }

        public async Task<string> PunchIntoJob()
        {
            string[] requiredParams = { "action", "employeeId", "currentJobId", "newJobId" };
            return await CallWebService(requiredParams);
        }

        public async Task<string> GetTodaysPunchesByEmployeeId()
        {
            string[] requiredParams = { "action", "employeeId" };
            return await CallWebService(requiredParams);
        }

        public async Task<string> GetThisWeeksPunchesByEmployeeId()
        {
            string[] requiredParams = { "action", "employeeId" };
            return await CallWebService(requiredParams);
        }


        private async Task<string> CallWebService(string[] requiredParams = null)
        {
            if (true == CommonMethods.CheckForRequiredParams(requiredParams, ParamDictionary) &&
                null != Employee
                )
            { return await CommonMethods.ReturnStringFromWebService(ParamDictionary); }
            else
            { return null; }
        }
    }

    class EmployeePunches
    {
        public RegularPunchPaired RegularPunchesPaired { get; set; }
        public RegularPunchOpen RegularPunchesOpen { get; set; }
        public JobPunchesPaired JobPunchesPaired { get; set; }
        public JobPunchOpen JobPunchesOpen { get; set; }
    }

    class EmployeePunchesForTheWeek
    {
        public List<WeekdayPunch> DayOfWeekPunch { get; set; }
    }

    class WeekdayPunch
    {
        public DateTime? Date { get; set; }
        public string DayName { get; set; }
        public List<RegularPunchPaired> RegularPunchesPaired { get; set; }
        public List<JobPunchesPaired> JobPunchesPaired { get; set; }
        public List<RegularPunchOpen> RegularPunchesOpen { get; set; }
        public List<JobPunchOpen> JobPunchesOpen { get; set; }
    }

    class RegularPunchPaired
    {
        public double? TotalDurationInSeconds { get; set; }
        public List<RegularPunch> Punches { get; set; }
    }

    class RegularPunch
    {
        public string ParentId { get; set; }
        public string ChildId { get; set; }
        public DateTime? PunchIn { get; set; }
        public DateTime? PunchOut { get; set; }
        public double? DurationInSeconds { get; set; }
    }

    class RegularPunchOpen
    {
        public string OpenId { get; set; }
        public string ParentId { get; set; }
        public DateTime? PunchIn { get; set; }
    }

    class JobPunchesPaired
    {
        public double? TotalDurationInSeconds { get; set; }
        public List<JobPunch> Punches { get; set; }
    }

    class JobPunch
    {
        public string ParentId { get; set; }
        public string ChildId { get; set; }
        public DateTime? PunchIn { get; set; }
        public DateTime? PunchOut { get; set; }
        public double? DurationInSeconds { get; set; }
        public JobInformation JobInformation { get; set; }
        //public string JobId { get; set; }
        //public double? JobDescription { get; set; }
    }

    class JobPunchOpen
    {
        public string OpenId { get; set; }
        public string ParentId { get; set; }
        public DateTime? PunchIn { get; set; }
        //public string JobId { get; set; }
        //public string JobDescription { get; set; }
        public JobInformation JobInformation { get; set; }
    }

    class JobInformation
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }


}
