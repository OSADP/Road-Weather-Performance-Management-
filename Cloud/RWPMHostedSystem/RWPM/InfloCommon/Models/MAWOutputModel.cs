using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class MAWOutputModel
    {
            public string alert_request_time { get; set; }
            public int alert_code_precip { get; set; }
            public int alert_action_code { get; set; }
            public string alert_time { get; set; }
            public int alert_code_pavement { get; set; }
            public int alert_code_visibility { get; set; }
            public string alert_gen_time { get; set; }
    }
}
