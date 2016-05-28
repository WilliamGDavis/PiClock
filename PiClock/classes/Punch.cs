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
        public List<RegularPunchesPaired> RegularPunchesPaired { get; set; }
        public List<RegularPunchesOpen> RegularPunchesOpen { get; set; }
        public List<JobPunchesPaired> JobPunchesPaired { get; set; }
        public List<JobPunchesOpen> JobPunchesOpen{ get; set; }
    }

    class EmployeePunchesForTheWeek
    {
        public List<WeekdayPunch> WeekdayPunches { get; set; }
    }

    class WeekdayPunch
    {
        public string DayName { get; set; }
        public double? DurationInSeconds { get; set; }
        //public DateTime? MaxPunch { get; set; }
    }

    class RegularPunchesPaired
    {
        public string Id_ParentPunch { get; set; }
        public string Id_ChildPunch { get; set; }
        public DateTime DateTimeIn { get; set; }
        public DateTime DateTimeOut { get; set; }
    }

    class RegularPunchesOpen
    {
        public string Id { get; set; }
        public string Id_ParentPunch { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
    }

    class JobPunchesPaired
    {
        public string Id_ParentPunch { get; set; }
        public string Id_ChildPunch { get; set; }
        public string JobDescription { get; set; }
        public DateTime DateTimeIn { get; set; }
        public DateTime DateTimeOut { get; set; }
    }

    class JobPunchesOpen
    {
        public string Id { get; set; }
        public string Id_Jobs { get; set; }
        public string JobDescription { get; set; }
        public string Id_ParentPunch { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
    }


}
