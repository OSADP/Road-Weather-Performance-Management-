using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace INFLOClassLib
{
    public static class clsGlobalVars
    {
        private static string m_DefaultErrContact = "h-charara@tamu.edu";
        private static string m_INFLOConfigFile = @"C:\Users\H-Charara\Documents\INFLO\Config\INFLOConfig.xml";
        private static string m_RoadwayLinkConfigFile = @"C:\Users\H-Charara\Documents\INFLO\Config\I-5-NB\RoadwayLinkConfiguration.csv";
        private static string m_DetectorStationConfigFile = @"C:\Users\H-Charara\Documents\INFLO\Config\I-5-NB\DetectionStationConfiguration.csv";
        private static string m_DetectionZoneConfigFile = @"C:\Users\H-Charara\Documents\INFLO\Config\I-5-NB\DetectionZoneConfiguration.csv";
        private static string m_DetectionZoneCombinedConfigFile = @"C:\Users\H-Charara\Documents\INFLO\Config\INFLO_SeattleDeployment_DetectionZoneCombinedConfigFile.CSV";
        private static string m_MileMarkerConfigFile = @"C:\Users\H-Charara\Documents\INFLO\Config\I-5-NB\MileMarkerConfiguration.csv";

        private static int m_MinimumDisplaySpeed = 30;
        private static int m_TroupeRange = 5;
        private static int m_3GantrySpeed = 15;
        private static int m_DSD = 30;  //Decision sight distance in seconds

        private static string m_CVDataDirectory;
        private static int m_CVDataPollingFrequency;
        private static int m_CVDataLoadingFrequency;
        private static int m_CVDataCurrInterval;
        private static int m_CVDataSmoothedSpeedIndex;
        private static int m_CVDataSmoothedSpeedArraySize;

        //New TSS data variables added for Seattle Demo
        //The local date and time that corresponds to the most recent WSDOT20 sec data file available for download
        //from the WSDOT 20 second data web site: @"http://data.wsdot.wa.gov/traffic/NW/FlowData/20second/"
        private static DateTime m_TSSDataCurrDateTime; 
        //Total number of detection zones available in the WSDOT 20 second data file
        private static int m_TSSDataNumDetectionZones;
        //The name of the file containing the information about the detection zones included in the current WSDOT 20 second data file
        private static string m_WSDOTDetectionZonesDailyFileName; 
        //The following two variables are used to generate the proper file name for the latest WSDOT 20 second data file
        // to be downloaded from the @"http://data.wsdot.wa.gov/traffic/NW/FlowData/20second/" website
        private static string m_TSSDataCurrFileName;
        private static string m_TSSDataCurrWebFileName;
        //URL for the wsdot 20 second real-time TSS data
        private static string m_WSDOT20SecDataFilesURL = @"http://data.wsdot.wa.gov/traffic/NW/FlowData/20second/";
        //End of variables added for Seattle demo

        private static int m_TSSDataCurrInterval;
        private static string m_TSSDataDirectory;
        private static int m_TSSDataPollingFrequency;
        private static int m_TSSDataLoadingFrequency;

        private static int m_ESSDataPollingFrequency = 5;          //in minutes
        private static int m_MobileESSDataPollingFrequency = 15;   // in minutes

        private static int m_LinkQueuedSpeedThreshold = 30;        //mph
        private static int m_LinkCongestedSpeedThreshold = 50;   //mph
        private static int m_SubLinkPercentQueuedCV = 50;
        private static double m_SubLinkLength = 0.1;   // miles
        private static double m_LinkLength = 0.5;   // miles

        private static double m_VehLength = 25;
        private static long m_CurrIntervalNum = 0;
        private static string m_TodayDirectoryName;

        private static string m_DBInterfaceType;
        private static string m_DBConnection;
        private static string m_AccessDBFileName;
        private static string m_SqlServer;
        private static string m_SqlServerDatabase;
        private static string m_SqlServerUserId;
        private static string m_SqlServerPassword;
        private static string m_SqlStrConnection;
        private static string m_DSNName;

        private static bool m_InfrastructureDataAvailable = true;
        private static double m_InfrastructureBOQLinkSpeed = 0;
        private static double m_InfrastructureBOQMMLocation = 0;
        private static DateTime m_InfrastructureBOQTime;
        private static double m_PrevInfrastructureBOQMMLocation = 0;
        private static DateTime m_PrevInfrastructureBOQTime;
        private static int m_TSSNumberNOQueueIntervals;

        private static bool m_CVDataAvailable = true;
        private static double m_CVBOQMMLocation = 0;
        private static double m_CVBOQSublinkSpeed;
        private static DateTime m_CVBOQTime;
        private static double m_PrevCVBOQMMLocation = 0;
        private static DateTime m_PrevCVBOQTime;
        private static int m_CVNumberNOQueueIntervals;

        private static double m_BOQMMLocation = 0;
        private static double m_BOQSpeed = 0;
        private static DateTime m_BOQTime;
        private static double m_PrevBOQMMLocation = 0;
        private static DateTime m_PrevBOQTime;

        private static double m_QueueSpeed;
        private static double m_QueueRate;
        private static double m_QueueLength;
        private static clsEnums.enQueueCahnge m_QueueChange;
        private static clsEnums.enQueueSource m_QueueSource;

        public static int CVDataSmoothedSpeedIndex
        {
            get { return m_CVDataSmoothedSpeedIndex; }
            set { m_CVDataSmoothedSpeedIndex = value; }
        }
        public static int CVDataSmoothedSpeedArraySize
        {
            get { return m_CVDataSmoothedSpeedArraySize; }
            set { m_CVDataSmoothedSpeedArraySize = value; }
        }
        public static string DBInterfaceType
        {
            get { return m_DBInterfaceType; }
            set { m_DBInterfaceType = value; }
        }
        public static string DBConnection
        {
            get { return m_DBConnection; }
            set { m_DBConnection = value; }
        }
        public static string AccessDBFileName
        {
            get { return m_AccessDBFileName; }
            set { m_AccessDBFileName = value; }
        }
        public static string SqlServer
        {
            get { return m_SqlServer; }
            set { m_SqlServer = value; }
        }
        public static string SqlServerDatabase
        {
            get { return m_SqlServerDatabase; }
            set { m_SqlServerDatabase = value; }
        }
        public static string SqlServerUserId
        {
            get { return m_SqlServerUserId; }
            set { m_SqlServerUserId = value; }
        }
        public static string SqlServerPassword
        {
            get { return m_SqlServerPassword; }
            set { m_SqlServerPassword = value; }
        }
        public static string SqlStrConnection
        {
            get { return m_SqlStrConnection; }
            set { m_SqlStrConnection = value; }
        }
        public static string DSNName
        {
            get { return m_DSNName; }
            set { m_DSNName = value; }
        }

        public static int MinimumDisplaySpeed
        {
            get { return m_MinimumDisplaySpeed; }
            set { m_MinimumDisplaySpeed = value; }
        }
        public static int DSD
        {
            get { return m_DSD; }
            set { m_DSD = value; }
        }
        public static int TroupeRange
        {
            get { return m_TroupeRange; }
            set { m_TroupeRange = value; }
        }
        public static int ThreeGantrySpeed
        {
            get { return m_3GantrySpeed; }
            set { m_3GantrySpeed = value; }
        }
        public static double VehLength
        {
            get { return m_VehLength; }
            set { m_VehLength = value; }
        }

        public static long CurrIntervalNum
        {
            get { return m_CurrIntervalNum; }
            set { m_CurrIntervalNum = value; }
        }
        public static string TodayDirectoryName
        {
            get { return m_TodayDirectoryName; }
            set { m_TodayDirectoryName = value; }
        }

        public static string INFLOConfigFile
        {
            get { return m_INFLOConfigFile; }
            set { m_INFLOConfigFile = value; }
        }
        public static string RoadwayLinkConfigFile
        {
            get { return m_RoadwayLinkConfigFile; }
            set { m_RoadwayLinkConfigFile = value; }
        }
        public static string DetectorStationConfigFile
        {
            get { return m_DetectorStationConfigFile; }
            set { m_DetectorStationConfigFile = value; }
        }
        public static string DetectionZoneConfigFile
        {
            get { return m_DetectionZoneConfigFile; }
            set { m_DetectionZoneConfigFile = value; }
        }
        public static string DetectionZoneCombinedConfigFile
        {
            get { return m_DetectionZoneCombinedConfigFile; }
            set { m_DetectionZoneCombinedConfigFile = value; }
        }
        public static string MileMarkerConfigFile
        {
            get { return m_MileMarkerConfigFile; }
            set { m_MileMarkerConfigFile = value; }
        }
        public static string DefaultErrContact
        {
            get { return m_DefaultErrContact; }
            set { m_TodayDirectoryName = value; }
        }

        public static int CVDataPollingFrequency
        {
            get { return m_CVDataPollingFrequency; }
            set { m_CVDataPollingFrequency = value; }
        }
        public static int CVDataLoadingFrequency
        {
            get { return m_CVDataLoadingFrequency; }
            set { m_CVDataLoadingFrequency = value; }
        }
        public static int CVDataCurrInterval
        {
            get { return m_CVDataCurrInterval; }
            set { m_CVDataCurrInterval = value; }
        }
        public static string CVDataDirectory
        {
            get { return m_CVDataDirectory; }
            set { m_CVDataDirectory = value; }
        }

        public static int TSSDataPollingFrequency
        {
            get { return m_TSSDataPollingFrequency; }
            set { m_TSSDataPollingFrequency = value; }
        }
        public static int TSSDataCurrInterval
        {
            get { return m_TSSDataCurrInterval; }
            set { m_TSSDataCurrInterval = value; }
        }
        public static DateTime TSSDataCurrDateTime
        {
            get { return m_TSSDataCurrDateTime; }
            set { m_TSSDataCurrDateTime = value; }
        }
        public static int TSSDataNumDetectionZones
        {
            get { return m_TSSDataNumDetectionZones; }
            set { m_TSSDataNumDetectionZones = value; }
        }
        public static string WSDOTDetectionZonesDailyFileName
        {
            get { return m_WSDOTDetectionZonesDailyFileName; }
            set { m_WSDOTDetectionZonesDailyFileName = value; }
        }
        public static string TSSDataCurrFileName
        {
            get { return m_TSSDataCurrFileName; }
            set { m_TSSDataCurrFileName = value; }
        }
        public static string TSSDataCurrWebFileName
        {
            get { return m_TSSDataCurrWebFileName; }
            set { m_TSSDataCurrWebFileName = value; }
        }
        public static string WSDOT20SecDataFilesURL
        {
            get { return m_WSDOT20SecDataFilesURL; }
            set { m_WSDOT20SecDataFilesURL = value; }
        }
        public static int TSSDataLoadingFrequency
        {
            get { return m_TSSDataLoadingFrequency; }
            set { m_TSSDataLoadingFrequency = value; }
        }
        public static string TSSDataDirectory
        {
            get { return m_TSSDataDirectory; }
            set { m_TSSDataDirectory = value; }
        }

        public static int ESSDataPollingFrequency
        {
            get { return m_ESSDataPollingFrequency; }
            set { m_ESSDataPollingFrequency = value; }
        }
        public static int MobileESSDataPollingFrequency
        {
            get { return m_MobileESSDataPollingFrequency; }
            set { m_MobileESSDataPollingFrequency = value; }
        }
        public static int LinkQueuedSpeedThreshold
        {
            get { return m_LinkQueuedSpeedThreshold; }
            set { m_LinkQueuedSpeedThreshold = value; }
        }
        public static int LinkCongestedSpeedThreshold
        {
            get { return m_LinkCongestedSpeedThreshold; }
            set { m_LinkCongestedSpeedThreshold = value; }
        }
        public static double SubLinkLength
        {
            get { return m_SubLinkLength; }
            set { m_SubLinkLength = value; }
        }
        public static double LinkLength
        {
            get { return m_LinkLength; }
            set { m_LinkLength = value; }
        }
        public static int SubLinkPercentQueuedCV
        {
            get { return m_SubLinkPercentQueuedCV; }
            set { m_SubLinkPercentQueuedCV = value; }
        }

        public static bool InfrastructureDataAvailable
        {
            get { return m_InfrastructureDataAvailable; }
            set { m_InfrastructureDataAvailable = value; }
        }
        public static bool CVDataAvailable
        {
            get { return m_CVDataAvailable; }
            set { m_CVDataAvailable = value; }
        }

        public static double InfrastructureBOQMMLocation
        {
            get { return m_InfrastructureBOQMMLocation; }
            set { m_InfrastructureBOQMMLocation = value; }
        }
        public static double InfrastructureBOQLinkSpeed
        {
            get { return m_InfrastructureBOQLinkSpeed; }
            set { m_InfrastructureBOQLinkSpeed = value; }
        }
        public static DateTime InfrastructureBOQTime
        {
            get { return m_InfrastructureBOQTime; }
            set { m_InfrastructureBOQTime = value; }
        }
        public static double PrevInfrastructureBOQMMLocation
        {
            get { return m_PrevInfrastructureBOQMMLocation; }
            set { m_PrevInfrastructureBOQMMLocation = value; }
        }
        public static DateTime PrevInfrastructureBOQTime
        {
            get { return m_PrevInfrastructureBOQTime; }
            set { m_PrevInfrastructureBOQTime = value; }
        }
        public static int TSSNumberNOQueueIntervals
        {
            get { return m_TSSNumberNOQueueIntervals; }
            set { m_TSSNumberNOQueueIntervals = value; }
        }

        public static double CVBOQMMLocation
        {
            get { return m_CVBOQMMLocation; }
            set { m_CVBOQMMLocation = value; }
        }
        public static double CVBOQSublinkSpeed
        {
            get { return m_CVBOQSublinkSpeed; }
            set { m_CVBOQSublinkSpeed = value; }
        }
        public static DateTime CVBOQTime
        {
            get { return m_CVBOQTime; }
            set { m_CVBOQTime = value; }
        }
        public static double PrevCVBOQMMLocation
        {
            get { return m_PrevCVBOQMMLocation; }
            set { m_PrevCVBOQMMLocation = value; }
        }
        public static DateTime PrevCVBOQTime
        {
            get { return m_PrevCVBOQTime; }
            set { m_PrevCVBOQTime = value; }
        }
        public static int CVNumberNOQueueIntervals
        {
            get { return m_CVNumberNOQueueIntervals; }
            set { m_CVNumberNOQueueIntervals = value; }
        }

        public static double BOQMMLocation
        {
            get { return m_BOQMMLocation; }
            set { m_BOQMMLocation = value; }
        }
        public static double BOQSpeed
        {
            get { return m_BOQSpeed; }
            set { m_BOQSpeed = value; }
        }
        public static DateTime BOQTime
        {
            get { return m_BOQTime; }
            set { m_BOQTime = value; }
        }
        public static double PrevBOQMMLocation
        {
            get { return m_PrevBOQMMLocation; }
            set { m_PrevBOQMMLocation = value; }
        }
        public static DateTime PrevBOQTime
        {
            get { return m_PrevBOQTime; }
            set { m_PrevBOQTime = value; }
        }
        public static double QueueSpeed
        {
            get { return m_QueueSpeed; }
            set { m_QueueSpeed = value; }
        }
        public static double QueueRate
        {
            get { return m_QueueRate; }
            set { m_QueueRate = value; }
        }
        public static double QueueLength
        {
            get { return m_QueueLength; }
            set { m_QueueLength = value; }
        }
        public static clsEnums.enQueueCahnge QueueChange
        {
            get { return m_QueueChange; }
            set { m_QueueChange = value; }
        }
        public static clsEnums.enQueueSource QueueSource
        {
            get { return m_QueueSource; }
            set { m_QueueSource = value; }
        }
        

        private static double m_MaximumDisplaySpeed;
        private static double m_COFLowerThreshold;
        private static double m_COFUpperThreshold;
        private static double m_RoadSurfaceStatusDryCOF;
        private static double m_RoadSurfaceStatusSnowCOF;
        private static double m_RoadSurfaceStatusWetCOF;
        private static double m_RoadSurfaceStatusIceCOF;
        private static double m_WRTMMaxRecommendedSpeed;
        private static double m_WRTMRecommendedSpeedLevel1;
        private static double m_WRTMRecommendedSpeedLevel2;
        private static double m_WRTMRecommendedSpeedLevel3;
        private static double m_WRTMRecommendedSpeedLevel4;
        private static double m_WRTMMinRecommendedSpeed;
        private static double m_VisibilityThreshold;
        private static double m_VisibilityStatusClear;
        private static double m_VisibilityStatusFogNotPatchy;
        private static double m_VisibilityStatusPatchyFog;
        private static double m_VisibilityStatusBlowingSnow;
        private static double m_VisibilityStatusSmoke;
        private static double m_VisibilityStatusSeaSpray;
        private static double m_VisibilityStatusVehicleSpray;
        private static double m_VisibilityStatusBlowingDust;
        private static double m_VisibilityStatusBlowingSand;
        private static double m_VisibilityStatusSunGlare;
        private static double m_VisibilityStatusSwarmsOfInsects;
        private static double m_OccupancyThreshold;
        private static double m_VolumeThreshold;
        private static double m_WRTMBeginMM = 2;
        private static double m_WRTMEndMM = 12.25;

        public static double WRTMBeginMM
        {
            get { return m_WRTMBeginMM; }
            set { m_WRTMBeginMM = value; }
        }
        public static double WRTMEndMM
        {
            get { return m_WRTMEndMM; }
            set { m_WRTMEndMM = value; }
        }
        public static double MaximumDisplaySpeed
        {
            get { return m_MaximumDisplaySpeed; }
            set { m_MaximumDisplaySpeed = value; }
        }
        public static double COFLowerThreshold
        {
            get { return m_COFLowerThreshold; }
            set { m_COFLowerThreshold = value; }
        }
        public static double COFUpperThreshold
        {
            get { return m_COFUpperThreshold; }
            set { m_COFUpperThreshold = value; }
        }
        public static double RoadSurfaceStatusDryCOF
        {
            get { return m_RoadSurfaceStatusDryCOF; }
            set { m_RoadSurfaceStatusDryCOF = value; }
        }
        public static double RoadSurfaceStatusSnowCOF
        {
            get { return m_RoadSurfaceStatusSnowCOF; }
            set { m_RoadSurfaceStatusSnowCOF = value; }
        }
        public static double RoadSurfaceStatusWetCOF
        {
            get { return m_RoadSurfaceStatusWetCOF; }
            set { m_RoadSurfaceStatusWetCOF = value; }
        }
        public static double RoadSurfaceStatusIceCOF
        {
            get { return m_RoadSurfaceStatusIceCOF; }
            set { m_RoadSurfaceStatusIceCOF = value; }
        }
        public static double WRTMMaxRecommendedSpeed
        {
            get { return m_WRTMMaxRecommendedSpeed; }
            set { m_WRTMMaxRecommendedSpeed = value; }
        }
        public static double WRTMRecommendedSpeedLevel1
        {
            get { return m_WRTMRecommendedSpeedLevel1; }
            set { m_WRTMRecommendedSpeedLevel1 = value; }
        }
        public static double WRTMRecommendedSpeedLevel2
        {
            get { return m_WRTMRecommendedSpeedLevel2; }
            set { m_WRTMRecommendedSpeedLevel2 = value; }
        }
        public static double WRTMRecommendedSpeedLevel3
        {
            get { return m_WRTMRecommendedSpeedLevel3; }
            set { m_WRTMRecommendedSpeedLevel3 = value; }
        }
        public static double WRTMRecommendedSpeedLevel4
        {
            get { return m_WRTMRecommendedSpeedLevel4; }
            set { m_WRTMRecommendedSpeedLevel4 = value; }
        }
        public static double WRTMMinRecommendedSpeed
        {
            get { return m_WRTMMinRecommendedSpeed; }
            set { m_WRTMMinRecommendedSpeed = value; }
        }
        public static double VisibilityThreshold
        {
            get { return m_VisibilityThreshold; }
            set { m_VisibilityThreshold = value; }
        }
        public static double VisibilityStatusClear
        {
            get { return m_VisibilityStatusClear; }
            set { m_VisibilityStatusClear = value; }
        }
        public static double VisibilityStatusFogNotPatchy
        {
            get { return m_VisibilityStatusFogNotPatchy; }
            set { m_VisibilityStatusFogNotPatchy = value; }
        }
        public static double VisibilityStatusPatchyFog
        {
            get { return m_VisibilityStatusPatchyFog; }
            set { m_VisibilityStatusPatchyFog = value; }
        }
        public static double VisibilityStatusBlowingSnow
        {
            get { return m_VisibilityStatusBlowingSnow; }
            set { m_VisibilityStatusBlowingSnow = value; }
        }
        public static double VisibilityStatusSmoke
        {
            get { return m_VisibilityStatusSmoke; }
            set { m_VisibilityStatusSmoke = value; }
        }
        public static double VisibilityStatusSeaSpray
        {
            get { return m_VisibilityStatusSeaSpray; }
            set { m_VisibilityStatusSeaSpray = value; }
        }
        public static double VisibilityStatusVehicleSpray
        {
            get { return m_VisibilityStatusVehicleSpray; }
            set { m_VisibilityStatusVehicleSpray = value; }
        }
        public static double VisibilityStatusBlowingDust
        {
            get { return m_VisibilityStatusBlowingDust; }
            set { m_VisibilityStatusBlowingDust = value; }
        }
        public static double VisibilityStatusBlowingSand
        {
            get { return m_VisibilityStatusBlowingSand; }
            set { m_VisibilityStatusBlowingSand = value; }
        }
        public static double VisibilityStatusSunGlare
        {
            get { return m_VisibilityStatusSunGlare; }
            set { m_VisibilityStatusSunGlare = value; }
        }
        public static double VisibilityStatusSwarmsOfInsects
        {
            get { return m_VisibilityStatusSwarmsOfInsects; }
            set { m_VisibilityStatusSwarmsOfInsects = value; }
        }
        public static double OccupancyThreshold
        {
            get { return m_OccupancyThreshold; }
            set { m_OccupancyThreshold = value; }
        }
        public static double VolumeThreshold
        {
            get { return m_VolumeThreshold; }
            set { m_VolumeThreshold = value; }
        }
    }
}
