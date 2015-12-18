using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITOMobileCommon.Models
{
    public class ExtendedProbe
    {
        public string NomadicDeviceId { get; set; }
        public System.DateTime DateGenerated { get; set; }
        public Nullable<double> AirTemperature { get; set; }
        public Nullable<double> AtmosphericPressure { get; set; }
        /// <summary>
        /// "On" == true; Optional.
        /// </summary>
        public string HeadlightStatus { get; set; }
        public double Speed { get; set; }
        public Nullable<double> SteeringWheelAngle { get; set; }
        /// <summary>
        /// "On" == true; Optional.
        /// </summary>
        public string WiperStatus { get; set; }
        public Nullable<double> RightFrontWheelSpeed { get; set; }
        public Nullable<double> LeftFrontWheelSpeed { get; set; }
        public Nullable<double> LeftRearWheelSpeed { get; set; }
        public Nullable<double> RightRearWheelSpeed { get; set; }
        public double GpsHeading { get; set; }
        public double GpsLatitude { get; set; }
        public double GpsLongitude { get; set; }
        public double GpsElevation { get; set; }
        public double GpsSpeed { get; set; }
        public Nullable<bool> CVQueuedStatus { get; set; }
    }
}
