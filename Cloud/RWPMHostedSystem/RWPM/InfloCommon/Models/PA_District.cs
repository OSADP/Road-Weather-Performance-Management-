using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class PA_District
    {
        public string district_name { get; set; }
        public string hr06_alert_summary_code { get; set; }
        public string hr24_alert_summary_code { get; set; }
        public string hr72_alert_summary_code { get; set; }
        public double max_lat { get; set; }
        public double max_lon { get; set; }
        public double min_lat { get; set; }
        public double min_lon { get; set; }
        public string obs_alert_summary_code { get; set; }
        public List<PA_Site> sites { get; set; }
    }
}
