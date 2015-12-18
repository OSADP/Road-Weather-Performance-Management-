using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITOMobileCommon.Models
{
    public class Coordinates
    {
        public Coordinates()
        { 
            DateTime_UTC = DateTime.UtcNow;
        }

        public Coordinates(double latitude, double longitude, double altitude, double accuracy, double heading, double speed)
        {
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
            Altitude = altitude;
            Heading = heading;
            Speed = speed;

            DateTime_UTC = DateTime.UtcNow;
        }
        public DateTime DateTime_UTC { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public double Altitude { get; set; }
        public double Heading { get; set; }
        public double Speed { get; set; }
    }
}
