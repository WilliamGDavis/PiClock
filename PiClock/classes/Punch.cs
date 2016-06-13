using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiClock.classes
{
    public class Punch
    {
        /**
        <summary>
            Lookup a JobId using the JobDescription
        </summary>
        <returns>
            string jobId (Job exists in the database)
            string "" (Job does not exist in the database)
            null (If deserialization error)
        </returns>
        */
        public static async Task<string> JobLookup(string jobDescription)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                {"action", "GetJobIdByJobDescription" },
                {"jobDescription", jobDescription }
            };
            var httpResponse = await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
            var httpContent = await httpResponse.Content.ReadAsStringAsync();

            return (string)CommonMethods.Deserialize(typeof(string), httpContent);
        }

        /**
        <summary>
            Punch a user out of an old job (if they're currently punched in) and punch them into a new job
        </summary>
        <returns>
            string "true"/"false"
            null (If deserialization error)
        </returns>
        */
        public static async Task PunchIntoJob(Employee employee, string newJobId)
        {
            string currentJobId = (null != employee.CurrentJob) ? employee.CurrentJob.Id.ToString() : "null";
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PunchIntoJob" },
                { "employeeId", employee.id },
                { "currentJobId",  currentJobId },
                { "newJobId", newJobId }
            };
            
            await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
        }

        /**
        <summary>
            Attempts to punch an employee into the database.  If successful, return a "true".  If unsuccessful, return "false". 
            Also, if an employee is currently punched into the database, return "false".
        </summary>
        <returns>
            string "true"/"false"
            null (If deserialization error)
        </returns>
        */
        public static async Task<bool> PunchIn(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PunchIn" },
                { "employeeId", employeeId }
            };
            
            var httpResponse = await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
            var httpContent = (string)CommonMethods.Deserialize(typeof(string), await httpResponse.Content.ReadAsStringAsync());

            return ("true" == httpContent) ? true : false;
        }

        /**
        <summary>
            Attempts to punch an employee out of the database, and any open Jobs (if currently punched into one)
            If successful, return a "true".  If unsuccessful, return "false". 
            Also, if an employee is currently punched into the database, return "false".
        </summary>
        <returns>
            string "true"/"false"
            null (If deserialization error)
        </returns>
        */
        public static async Task<bool> PunchOut(string employeeId, string currentJobId)
        {
            var punch = new Punch();
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PunchOut" },
                { "employeeId", employeeId },
                { "currentJobId", currentJobId }
            };

            var httpResponse = await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
            var httpContent = (string)CommonMethods.Deserialize(typeof(string), await httpResponse.Content.ReadAsStringAsync());

            return ("true" == httpContent) ? true : false;
        }

        /**
        <summary>
            Return an employee's punches for a given day (Currently set for "today" only)
        </summary>
        <returns>
            array
            null (If deserialization error)
        </returns>
        */
        public static async Task<EmployeePunchesByDay> GetTodaysPunches(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetSingleDayPunchesByEmployeeId" },
                { "employeeId", employeeId }
            };

            var httpResponse = await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
            var httpContent = await httpResponse.Content.ReadAsStringAsync();

            return (EmployeePunchesByDay)CommonMethods.Deserialize(typeof(EmployeePunchesByDay), httpContent);
        }

        /**
        <summary>
            Return an employee's punches for a given week (Currently set for "today" only)
        </summary>
        <returns>
            array
            null (If deserialization error)
        </returns>
        */
        public static async Task<EmployeePunchesByWeek> GetThisWeeksPunches(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetThisWeeksPunchesByEmployeeId" },
                { "employeeId", employeeId }
            };

            var httpResponse = await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
            var httpContent = await httpResponse.Content.ReadAsStringAsync();

            return (EmployeePunchesByWeek)CommonMethods.Deserialize(typeof(EmployeePunchesByWeek), httpContent);
        }
    }

    public class EmployeePunchesByDay
    {
        public RegularPunchOpen RegularPunchesOpen { get; set; }
        public JobPunchOpen JobPunchesOpen { get; set; }
        public RegularPunchesPaired RegularPunchesPaired { get; set; }
        public JobPunchesPaired JobPunchesPaired { get; set; }

    }

    public class EmployeePunchesByWeek
    {
        public List<PairedPunches> PairedPunches { get; set; }
        public OpenPunches OpenPunches { get; set; }
    }

    public class PairedPunches
    {
        public DateTime? Date { get; set; }
        public string DayName { get; set; }
        public List<RegularPunchesPaired> RegularPunchesPaired { get; set; }
        public List<JobPunchesPaired> JobPunchesPaired { get; set; }
    }

    public class OpenPunches
    {
        public RegularPunchOpen RegularPunchesOpen { get; set; }
        public JobPunchOpen JobPunchesOpen { get; set; }
    }

    public class WeekdayPunch
    {
        public DateTime? Date { get; set; }
        public string DayName { get; set; }
        public List<RegularPunchesPaired> RegularPunchesPaired { get; set; }
        public List<JobPunchesPaired> JobPunchesPaired { get; set; }
        public List<RegularPunchOpen> RegularPunchesOpen { get; set; }
        public List<JobPunchOpen> JobPunchesOpen { get; set; }
    }

    public class PairedPunch
    {
        public string ParentId { get; set; }
        public string ChildId { get; set; }
        public DateTime? PunchIn { get; set; }
        public DateTime? PunchOut { get; set; }
        public double? DurationInSeconds { get; set; }
    }

    public class OpenPunch
    {
        public string OpenId { get; set; }
        public string ParentId { get; set; }
        public DateTime? PunchIn { get; set; }
    }

    public class RegularPunchesPaired
    {
        public double? TotalDurationInSeconds { get; set; }
        public List<PairedPunch> Punches { get; set; }
    }

    public class JobPunchesPaired
    {
        public double? TotalDurationInSeconds { get; set; }
        public List<JobPunch> Punches { get; set; }
    }

    public class RegularPunch : PairedPunch { } //Not Used
    public class RegularPunchOpen : OpenPunch { }

    public class JobPunch : PairedPunch
    { public JobInformation JobInformation { get; set; } }

    public class JobPunchOpen : OpenPunch
    { public JobInformation JobInformation { get; set; } }

    public class JobInformation
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }


}
