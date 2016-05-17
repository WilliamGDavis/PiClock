using Newtonsoft.Json;
using System.Collections.Generic;

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
        [JsonProperty("currentJob")]
        public Job CurrentJob { get; set; }
        [JsonProperty("jobList")]
        public List<Job> JobList { get; set; }
    }
}
