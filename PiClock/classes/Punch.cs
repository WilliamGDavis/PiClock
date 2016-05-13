using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class Punch
    {

        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("id_users")]
        public string id_users {get; set;}
        [JsonProperty("id_jobs")]
        public string id_jobs { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }

        public void PunchIn(Employee employee)
        {

        }
    }
}
