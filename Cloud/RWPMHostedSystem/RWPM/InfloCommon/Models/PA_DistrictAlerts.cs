using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class PA_DistrictAlerts
    {
        public string data_time { get; set; }
        public List<PA_District> districts { get; set; }
    }
}
