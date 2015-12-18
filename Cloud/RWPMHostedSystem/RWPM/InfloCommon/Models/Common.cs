using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfloCommon.Models
{
    public static class Common
    {
        /// <summary>
        /// Oct2015 version uses 'ValidityDuration' of SpdHarm and QWarn alerts more
        /// as a status than a duration - e.g. the duration is never 'applied' to the current
        /// datetime - rather, 60 means it is active. 0 means the DataProcessor has since added
        /// more up-to-date data and invalidated the old, and -1 means an admin on the website
        /// invalidated the event manually.
        /// </summary>
        public static int INFLO_VALIDITY_DURATION_ACTIVE = 60;
        public static int INFLO_VALIDITY_DURATION_AUTO_INACTIVE = 0;
        public static int INFLO_VALIDITY_DURATION_MANUAL_INACTIVE = -1;
    }

    /// <summary>
    /// Enum maps to WeatherEvents.Mode database column. Didn't see the need to make a official lookup table for two values.
    /// </summary>
    public enum WeatherEventMode
    {
        Auto=1,
        Manual=2
    }
}
