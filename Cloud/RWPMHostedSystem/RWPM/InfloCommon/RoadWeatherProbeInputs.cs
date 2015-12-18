//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InfloCommon
{
    using System;
    using System.Collections.Generic;
    
    public partial class RoadWeatherProbeInputs
    {
        public int Id { get; set; }
        public string NomadicDeviceId { get; set; }
        public System.DateTime DateGenerated { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<double> AirTemperature { get; set; }
        public Nullable<double> AtmosphericPressure { get; set; }
        public Nullable<bool> HeadlightStatus { get; set; }
        public double Speed { get; set; }
        public Nullable<double> SteeringWheelAngle { get; set; }
        public Nullable<bool> WiperStatus { get; set; }
        public Nullable<double> RightFrontWheelSpeed { get; set; }
        public Nullable<double> LeftFrontWheelSpeed { get; set; }
        public Nullable<double> LeftRearWheelSpeed { get; set; }
        public Nullable<double> RightRearWheelSpeed { get; set; }
        public double GpsHeading { get; set; }
        public double GpsLatitude { get; set; }
        public double GpsLongitude { get; set; }
        public double GpsElevation { get; set; }
        public double GpsSpeed { get; set; }
    }
}