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
            var httpContent = await httpResponse.Content.ReadAsStringAsync();
            string punchIn = (string)CommonMethods.Deserialize(typeof(string), httpContent);

            return ("true" == punchIn) ? true : false;
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
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "PunchOut" },
                { "employeeId", employeeId },
                { "currentJobId", currentJobId }
            };

            var httpResponse = await CommonMethods.GetHttpResponseFromRpcServer(paramDictionary);
            var httpContent = await httpResponse.Content.ReadAsStringAsync();
            var punchOut = (string)CommonMethods.Deserialize(typeof(string), httpContent);

            return ("true" == punchOut) ? true : false;
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
            Return an employee's punches for a given range (Currently set for "beginning of the week and end of the week" only)
        </summary>
        <returns>
            array
            null (If deserialization error)
        </returns>
        */
        public static async Task<EmployeePunchesByDay> GetRangePunches(string employeeId)
        {
            var paramDictionary = new Dictionary<string, string>()
            {
                { "action", "GetRangePunchesByEmployeeId" },
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
        //public RegularPunchOpen RegularPunchesOpen { get; set; }
        //public JobPunchOpen JobPunchesOpen { get; set; }
        public RegularPunches RegularPunches { get; set; }
        public JobPunches JobPunches { get; set; }

    }

    public class EmployeePunchesByWeek
    {
        public List<WeekdayPunch> WeekdayPunches { get; set; }
        //public List<PairedPunches> PairedPunches { get; set; }
        //public OpenPunches OpenPunches { get; set; }
    }

    public class PairedPunches
    {
        public DateTime? Date { get; set; }
        public string DayName { get; set; }
        public List<RegularPunches> RegularPunchesPaired { get; set; }
        public List<JobPunches> JobPunchesPaired { get; set; }
    }

    public class OpenPunches
    {
        public RegularPunchOpen RegularPunchesOpen { get; set; }
        public JobPunchOpen JobPunchesOpen { get; set; }
    }

    public class WeekdayPunch
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public List<RegularPunches> RegularPunches { get; set; }
        public List<JobPunches> JobPunches { get; set; }
        //public List<RegularPunchOpen> RegularPunchesOpen { get; set; }
        //public List<JobPunchOpen> JobPunchesOpen { get; set; }
    }

    public class RegularPunch
    {
        public string Id { get; set; }
        public DateTime PunchIn { get; set; }
        public DateTime? PunchOut { get; set; } //Will sometimes be an empty string, if an employee is currently punched in
        public double? DurationInSeconds { get; set; } //Will sometimes be an empty string, if an employee is currently punched in
    }

    public class OpenPunch
    {
        public string OpenId { get; set; }
        public string ParentId { get; set; }
        public DateTime? PunchIn { get; set; }
    }

    public class RegularPunches
    {
        public double? TotalDurationInSeconds { get; set; }
        public List<RegularPunch> Punches { get; set; }
    }

    public class JobPunches
    {
        public double? TotalDurationInSeconds { get; set; }
        public List<JobPunch> Punches { get; set; }
    }

    //public class RegularPunch : PairedPunch { } //Not Used
    public class RegularPunchOpen : OpenPunch { }

    public class JobPunch : RegularPunch
    { public JobInformation JobInformation { get; set; } }

    public class JobPunchOpen : OpenPunch
    { public JobInformation JobInformation { get; set; } }

    public class JobInformation
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }


}
