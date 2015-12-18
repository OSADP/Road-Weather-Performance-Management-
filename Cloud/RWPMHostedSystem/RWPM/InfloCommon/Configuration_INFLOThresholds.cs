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
    
    public partial class Configuration_INFLOThresholds
    {
        public short CVDataPollingFrequency { get; set; }
        public short TSSDataPollingFrequency { get; set; }
        public Nullable<short> CVDataLoadingFrequency { get; set; }
        public Nullable<short> TSSDataLoadingFrequency { get; set; }
        public short ESSDataPollingFrequency { get; set; }
        public short MobileESSDataPollingFrequency { get; set; }
        public short QueuedSpeedThreshold { get; set; }
        public short CongestedSpeedThreshold { get; set; }
        public Nullable<short> MinimumDisplaySpeed { get; set; }
        public Nullable<short> TroupeRange { get; set; }
        public Nullable<short> ThreeGantrySpeed { get; set; }
        public Nullable<short> RecurringCongestionMMLocation { get; set; }
        public Nullable<double> SublinkLength { get; set; }
        public Nullable<short> SublinkPercentQueuedCV { get; set; }
        public Nullable<short> MaximumDisplaySpeed { get; set; }
        public Nullable<double> COFLowerThreshold { get; set; }
        public Nullable<double> COFUpperThreshold { get; set; }
        public Nullable<double> RoadSurfaceStatusDryCOF { get; set; }
        public Nullable<double> RoadSurfaceStatusSnowCOF { get; set; }
        public Nullable<double> RoadSurfaceStatusWetCOF { get; set; }
        public Nullable<double> RoadSurfaceStatusIceCOF { get; set; }
        public Nullable<double> WRTMMaxRecommendedSpeed { get; set; }
        public Nullable<double> WRTMMaxRecommendedSpeedLevel1 { get; set; }
        public Nullable<double> WRTMMaxRecommendedSpeedLevel2 { get; set; }
        public Nullable<double> WRTMMaxRecommendedSpeedLevel3 { get; set; }
        public Nullable<double> WRTMMaxRecommendedSpeedLevel4 { get; set; }
        public Nullable<double> WRTMMinRecommendedSpeed { get; set; }
        public Nullable<double> VisibilityThreshold { get; set; }
        public Nullable<double> VisibilityStatusClear { get; set; }
        public Nullable<double> VisibilityStatusFogNotPatchy { get; set; }
        public Nullable<double> VisibilityStatusPatchyFog { get; set; }
        public Nullable<double> VisibilityStatusBlowingSnow { get; set; }
        public Nullable<double> VisibilityStatusSmoke { get; set; }
        public Nullable<double> VisibilityStatusSeaSpray { get; set; }
        public Nullable<double> VisibilityStatusVehicleSpray { get; set; }
        public Nullable<double> VisibilityStatusBlowingDust { get; set; }
        public Nullable<double> VisibilityStatusBlowingSand { get; set; }
        public Nullable<double> VisibilityStatusSunGlare { get; set; }
        public Nullable<double> VisibilityStatusSwarmsOfInsects { get; set; }
        public Nullable<short> OccupancyThreshold { get; set; }
        public Nullable<short> VolumeThreshold { get; set; }
        public int Id { get; set; }
    }
}
