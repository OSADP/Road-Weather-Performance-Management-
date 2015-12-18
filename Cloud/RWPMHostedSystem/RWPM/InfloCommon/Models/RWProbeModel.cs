using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfloCommon.Models
{
    //Uses InfloWebRole.Models namespace by default. Need to define namespace on xml passed to api:
    //<RWProbeModel xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/Bsm.Models">
    //(Alternatively:To define a specific namespace, we modify the controller model. Need to add reference to System.Runtime.Serialization to make this work.

    //DateTime format for xml format must be like:2007-11-28T16:00:00.000Z

    //XML Auto binding requires fields in alphabetically order or else they will be null. json can be in any order. As follows:
    /*
   
<ArrayOfRWProbeModel xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/InfloCommon.Models">
<RWProbeModel>
<AirTemperature>500</AirTemperature>
<AtmosphericPressure>12.5</AtmosphericPressure>
<DateGenerated>2007-11-28T16:00:00.000Z</DateGenerated>
<GpsElevation>100</GpsElevation>
<GpsHeading>50</GpsHeading>
<GpsLatitude>12.5</GpsLatitude>
<GpsLongitude>12.5</GpsLongitude>
<GpsSpeed>12.5</GpsSpeed>
<HeadlightStatus>On</HeadlightStatus>
<LeftFrontWheelSpeed>58</LeftFrontWheelSpeed>
<LeftRearWheelSpeed>85.4</LeftRearWheelSpeed>
<NomadicDeviceId>new</NomadicDeviceId>
<RightFrontWheelSpeed>85</RightFrontWheelSpeed>
<RightRearWheelSpeed>85.2</RightRearWheelSpeed>
<Speed>12.5</Speed>
</RWProbeModel>
</ArrayOfRWProbeModel>
 
     Populate CVQueuedStatus if it should be parsed as a BSM Message and loaded to [TME_CVData_Input].
     [{"NomadicDeviceId":"771C23C6",
"DateGenerated":"2015-09-26 14:25:26.000",
"CVQueuedStatus":"false",
"LatAccel":"12",
"LongAccel":"22",
"Speed":"12.2",
"GpsHeading":"1",
"GpsLatitude":"16",
"GpsLongitude":"17",
"GpsElevation":"0",
"GpsSpeed":"65"}]
    
     * */

    public class RWProbeModel
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


     

        /// <summary>
        /// For BSM message support.  Indicates if the entry is queued.
        /// The presence of this field also dictates if this is a "BSM Message" type or a true Road Weather Probe type
        /// </summary>
        public Nullable<bool> CVQueuedStatus { get; set; }
        /// <summary>
        /// For BSM message support
        /// </summary>
        public Nullable<double> LatAccel { get; set; }
        /// <summary>
        /// For BSM message support
        /// </summary>
        public Nullable<double> LongAccel { get; set; }
        /// <summary>
        /// CVQueuedStatus != null.  The Probe queue is dual-use, it funnels data entries to both the BSM message path ([TME_CVData_Input])
        /// and to the RW Probe path ([RoadWeatherProbeInputs]). They are similar in fields but not identical.
        /// Queued status is only used for BSM (and required), thus is the determinator.
        /// </summary>
        /// <returns></returns>
        public bool IsBsm()
        {
            if (CVQueuedStatus != null) return true;
            else return false;
        }

    }
}