using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public class PA_Site
    {
        public string desc { get; set; }
        public string hr06_alert_code { get; set; }
        public string hr24_alert_code { get; set; }
        public string hr72_alert_code { get; set; }
        public bool is_road_cond_site { get; set; }
        public bool is_rwis_site { get; set; }
        public bool is_wx_obs_site { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string obs_alert_code { get; set; }
        public string site_id { get; set; }
        public int site_num { get; set; }
        public List<PA_Time_Series> time_series { get; set; }
    }
}
