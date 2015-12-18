using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class Pikalert_Vehicles
    {
        public string data_time { get; set; }
        public List<PA_Vehicles_District> districts { get; set; }
    }
}
