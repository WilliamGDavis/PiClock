using System;
using System.Collections.Generic;
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
            { return await CommonMethods.GetJsonFromRpcServer(ParamDictionary); }
            else
            { return null; }
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
