using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class PA_Time_Series
    {
        public string alert_code { get; set; }
        public string chemical { get; set; }
        public string pavement { get; set; }
        public string plow { get; set; }
        public string precip { get; set; }
        public int road_temp { get; set; }
        public string time { get; set; }
        public string treatment_alert_code { get; set; }
        public string visibility { get; set; }
    }
}
