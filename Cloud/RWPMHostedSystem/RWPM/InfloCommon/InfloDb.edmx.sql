
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/23/2015 09:21:58
-- Generated from EDMX file: D:\Projects\RWPM\SourceCode\Cloud\trunk\RWPMHostedSystem\RWPM\InfloCommon\InfloDb.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [inflo];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_EDMSSAlertDistrict]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Districts] DROP CONSTRAINT [FK_EDMSSAlertDistrict];
GO
IF OBJECT_ID(N'[dbo].[FK_DistrictSite]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sites] DROP CONSTRAINT [FK_DistrictSite];
GO
IF OBJECT_ID(N'[dbo].[FK_SiteSiteObservation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SiteObservations] DROP CONSTRAINT [FK_SiteSiteObservation];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Configuration_ESS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_ESS];
GO
IF OBJECT_ID(N'[dbo].[Configuration_INFLOThresholds]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_INFLOThresholds];
GO
IF OBJECT_ID(N'[dbo].[Configuration_Roadway]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_Roadway];
GO
IF OBJECT_ID(N'[dbo].[Configuration_RoadwayESS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_RoadwayESS];
GO
IF OBJECT_ID(N'[dbo].[Configuration_RoadwayLinks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_RoadwayLinks];
GO
IF OBJECT_ID(N'[dbo].[Configuration_RoadwayLinksDetectorStation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_RoadwayLinksDetectorStation];
GO
IF OBJECT_ID(N'[dbo].[Configuration_RoadwayLinksESS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_RoadwayLinksESS];
GO
IF OBJECT_ID(N'[dbo].[Configuration_RoadwayMileMarkers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_RoadwayMileMarkers];
GO
IF OBJECT_ID(N'[dbo].[Configuration_RoadwaySubLinks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_RoadwaySubLinks];
GO
IF OBJECT_ID(N'[dbo].[Configuration_TSSDetectionZone]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_TSSDetectionZone];
GO
IF OBJECT_ID(N'[dbo].[Configuration_TSSDetectorStation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration_TSSDetectorStation];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[TME_CVData_Input]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TME_CVData_Input];
GO
IF OBJECT_ID(N'[dbo].[TME_CVData_Input_Processed]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TME_CVData_Input_Processed];
GO
IF OBJECT_ID(N'[dbo].[TME_CVData_SubLink]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TME_CVData_SubLink];
GO
IF OBJECT_ID(N'[dbo].[TME_ESSData_Input]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TME_ESSData_Input];
GO
IF OBJECT_ID(N'[dbo].[TME_ESSMobileData_Input]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TME_ESSMobileData_Input];
GO
IF OBJECT_ID(N'[dbo].[TME_TSSData_Input]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TME_TSSData_Input];
GO
IF OBJECT_ID(N'[dbo].[TME_TSSESS_Link]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TME_TSSESS_Link];
GO
IF OBJECT_ID(N'[dbo].[TMEOutput_QWARN_QueueInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TMEOutput_QWARN_QueueInfo];
GO
IF OBJECT_ID(N'[dbo].[TMEOutput_QWARNMessage_CV]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TMEOutput_QWARNMessage_CV];
GO
IF OBJECT_ID(N'[dbo].[TMEOutput_ShockWaveInformation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TMEOutput_ShockWaveInformation];
GO
IF OBJECT_ID(N'[dbo].[TMEOutput_SPDHARMMessage_CV]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TMEOutput_SPDHARMMessage_CV];
GO
IF OBJECT_ID(N'[dbo].[TMEOutput_SPDHARMMessage_Infrastructure]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TMEOutput_SPDHARMMessage_Infrastructure];
GO
IF OBJECT_ID(N'[dbo].[TMEOutput_WRTM_Alerts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TMEOutput_WRTM_Alerts];
GO
IF OBJECT_ID(N'[dbo].[RoadWeatherProbeInputs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoadWeatherProbeInputs];
GO
IF OBJECT_ID(N'[dbo].[MAWOutputs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MAWOutputs];
GO
IF OBJECT_ID(N'[dbo].[WeatherEvents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WeatherEvents];
GO
IF OBJECT_ID(N'[dbo].[EDMSSAlerts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EDMSSAlerts];
GO
IF OBJECT_ID(N'[dbo].[Districts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Districts];
GO
IF OBJECT_ID(N'[dbo].[Sites]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sites];
GO
IF OBJECT_ID(N'[dbo].[SiteObservations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SiteObservations];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Configuration_ESS'
CREATE TABLE [dbo].[Configuration_ESS] (
    [ESSId] nvarchar(25)  NOT NULL,
    [ESSType] nvarchar(50)  NULL,
    [ESSOperationType] nvarchar(50)  NULL,
    [ESSMobilityType] nvarchar(25)  NULL,
    [BeginMM] float  NULL,
    [EndMM] float  NULL
);
GO

-- Creating table 'Configuration_INFLOThresholds'
CREATE TABLE [dbo].[Configuration_INFLOThresholds] (
    [CVDataPollingFrequency] smallint  NOT NULL,
    [TSSDataPollingFrequency] smallint  NOT NULL,
    [CVDataLoadingFrequency] smallint  NULL,
    [TSSDataLoadingFrequency] smallint  NULL,
    [ESSDataPollingFrequency] smallint  NOT NULL,
    [MobileESSDataPollingFrequency] smallint  NOT NULL,
    [QueuedSpeedThreshold] smallint  NOT NULL,
    [CongestedSpeedThreshold] smallint  NOT NULL,
    [MinimumDisplaySpeed] smallint  NULL,
    [TroupeRange] smallint  NULL,
    [ThreeGantrySpeed] smallint  NULL,
    [RecurringCongestionMMLocation] smallint  NULL,
    [SublinkLength] float  NULL,
    [SublinkPercentQueuedCV] smallint  NULL,
    [MaximumDisplaySpeed] smallint  NULL,
    [COFLowerThreshold] float  NULL,
    [COFUpperThreshold] float  NULL,
    [RoadSurfaceStatusDryCOF] float  NULL,
    [RoadSurfaceStatusSnowCOF] float  NULL,
    [RoadSurfaceStatusWetCOF] float  NULL,
    [RoadSurfaceStatusIceCOF] float  NULL,
    [WRTMMaxRecommendedSpeed] float  NULL,
    [WRTMMaxRecommendedSpeedLevel1] float  NULL,
    [WRTMMaxRecommendedSpeedLevel2] float  NULL,
    [WRTMMaxRecommendedSpeedLevel3] float  NULL,
    [WRTMMaxRecommendedSpeedLevel4] float  NULL,
    [WRTMMinRecommendedSpeed] float  NULL,
    [VisibilityThreshold] float  NULL,
    [VisibilityStatusClear] float  NULL,
    [VisibilityStatusFogNotPatchy] float  NULL,
    [VisibilityStatusPatchyFog] float  NULL,
    [VisibilityStatusBlowingSnow] float  NULL,
    [VisibilityStatusSmoke] float  NULL,
    [VisibilityStatusSeaSpray] float  NULL,
    [VisibilityStatusVehicleSpray] float  NULL,
    [VisibilityStatusBlowingDust] float  NULL,
    [VisibilityStatusBlowingSand] float  NULL,
    [VisibilityStatusSunGlare] float  NULL,
    [VisibilityStatusSwarmsOfInsects] float  NULL,
    [OccupancyThreshold] smallint  NULL,
    [VolumeThreshold] smallint  NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Configuration_Roadway'
CREATE TABLE [dbo].[Configuration_Roadway] (
    [RoadwayId] nvarchar(25)  NULL,
    [Name] nvarchar(25)  NOT NULL,
    [Direction] nvarchar(15)  NOT NULL,
    [Grade] float  NULL,
    [BeginMM] float  NOT NULL,
    [EndMM] float  NOT NULL,
    [MMIncreasingDirection] nvarchar(15)  NOT NULL,
    [LowerHeading] float  NULL,
    [UpperHeading] float  NULL,
    [RecurringCongestionMMLocation] float  NULL
);
GO

-- Creating table 'Configuration_RoadwayESS'
CREATE TABLE [dbo].[Configuration_RoadwayESS] (
    [RoadwayId] nvarchar(25)  NOT NULL,
    [ESSId] nvarchar(25)  NOT NULL
);
GO

-- Creating table 'Configuration_RoadwayLinks'
CREATE TABLE [dbo].[Configuration_RoadwayLinks] (
    [RoadwayId] nvarchar(25)  NOT NULL,
    [LinkId] nvarchar(25)  NOT NULL,
    [BeginMM] float  NOT NULL,
    [EndMM] float  NOT NULL,
    [StartCrossStreetName] nvarchar(50)  NULL,
    [EndCrossStreetName] nvarchar(50)  NULL,
    [UpstreamLinkId] nvarchar(25)  NULL,
    [DownstreamLinkId] nvarchar(25)  NULL,
    [NumberLanes] smallint  NULL,
    [SpeedLimit] smallint  NULL,
    [NumberDetectorStations] smallint  NULL,
    [DetectorStations] nvarchar(50)  NULL,
    [ESS] nvarchar(25)  NULL,
    [VSLSignId] nvarchar(25)  NULL,
    [DMSId] nvarchar(25)  NULL,
    [RSEId] nvarchar(25)  NULL,
    [Direction] nvarchar(15)  NULL
);
GO

-- Creating table 'Configuration_RoadwayLinksDetectorStation'
CREATE TABLE [dbo].[Configuration_RoadwayLinksDetectorStation] (
    [LinkId] nvarchar(25)  NOT NULL,
    [DSId] nvarchar(25)  NOT NULL
);
GO

-- Creating table 'Configuration_RoadwayLinksESS'
CREATE TABLE [dbo].[Configuration_RoadwayLinksESS] (
    [LinkId] nvarchar(25)  NOT NULL,
    [ESSId] nvarchar(25)  NOT NULL
);
GO

-- Creating table 'Configuration_RoadwayMileMarkers'
CREATE TABLE [dbo].[Configuration_RoadwayMileMarkers] (
    [RoadwayId] nvarchar(25)  NOT NULL,
    [MMNumber] float  NOT NULL,
    [Latitude1] float  NOT NULL,
    [Longitude1] float  NOT NULL,
    [Latitude2] float  NOT NULL,
    [Longitude2] float  NOT NULL
);
GO

-- Creating table 'Configuration_RoadwaySubLinks'
CREATE TABLE [dbo].[Configuration_RoadwaySubLinks] (
    [RoadwayId] nvarchar(25)  NOT NULL,
    [SubLinkId] nvarchar(25)  NOT NULL,
    [BeginMM] float  NOT NULL,
    [EndMM] float  NOT NULL,
    [SpeedLimit] smallint  NULL,
    [NumberLanes] smallint  NULL,
    [DownstreamSubLinkId] nvarchar(25)  NULL,
    [UpstreamSubLinkId] nvarchar(25)  NULL,
    [VSLSignID] nvarchar(25)  NULL,
    [DMSID] nvarchar(25)  NULL,
    [RSEID] nvarchar(25)  NULL,
    [Direction] nvarchar(15)  NULL
);
GO

-- Creating table 'Configuration_TSSDetectionZone'
CREATE TABLE [dbo].[Configuration_TSSDetectionZone] (
    [DSId] nvarchar(25)  NOT NULL,
    [DZId] nvarchar(25)  NOT NULL,
    [DZType] nvarchar(50)  NULL,
    [LaneNumber] int  NOT NULL,
    [LaneType] nvarchar(50)  NULL,
    [LaneDescription] nvarchar(100)  NULL,
    [DataType] nvarchar(25)  NULL,
    [DZStatus] nvarchar(25)  NULL,
    [Direction] nvarchar(3)  NULL,
    [RoadwayId] nvarchar(50)  NULL
);
GO

-- Creating table 'Configuration_TSSDetectorStation'
CREATE TABLE [dbo].[Configuration_TSSDetectorStation] (
    [LinkId] nvarchar(25)  NULL,
    [DSId] nvarchar(25)  NOT NULL,
    [DSName] nvarchar(50)  NULL,
    [MMLocation] float  NOT NULL,
    [NumberLanes] smallint  NULL,
    [NumberDetectionZones] smallint  NOT NULL,
    [DetectionZones] nvarchar(100)  NULL,
    [Latitude] float  NULL,
    [Longitude] float  NULL,
    [Direction] nvarchar(3)  NULL,
    [RoadwayId] nvarchar(50)  NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'TME_CVData_Input'
CREATE TABLE [dbo].[TME_CVData_Input] (
    [CVMessageIdentifier] bigint IDENTITY(1,1) NOT NULL,
    [NomadicDeviceID] nvarchar(25)  NULL,
    [DateGenerated] datetime  NULL,
    [Speed] float  NULL,
    [Heading] smallint  NULL,
    [Latitude] float  NULL,
    [Longitude] float  NULL,
    [MMLocation] float  NOT NULL,
    [CVQueuedState] bit  NULL,
    [CoefficientOfFriction] float  NULL,
    [Temperature] smallint  NULL,
    [RoadwayId] nvarchar(25)  NULL,
    [LateralAcceleration] float  NULL,
    [LongitudinalAcceleration] float  NULL
);
GO

-- Creating table 'TME_CVData_Input_Processed'
CREATE TABLE [dbo].[TME_CVData_Input_Processed] (
    [NomadicDeviceID] nvarchar(25)  NULL,
    [DateGenerated] nvarchar(25)  NULL,
    [DateProcessed] nvarchar(25)  NULL,
    [Speed] float  NULL,
    [Heading] smallint  NULL,
    [MMLocation] float  NOT NULL,
    [CVQueuedState] bit  NULL,
    [RoadwayID] nvarchar(25)  NULL,
    [CoefficientOfFriction] float  NULL,
    [Temperature] smallint  NULL,
    [SubLinkId] nvarchar(25)  NULL
);
GO

-- Creating table 'TME_CVData_SubLink'
CREATE TABLE [dbo].[TME_CVData_SubLink] (
    [RoadwayId] nvarchar(25)  NULL,
    [SubLinkId] nvarchar(25)  NOT NULL,
    [DateProcessed] datetime  NOT NULL,
    [IntervalLength] smallint  NULL,
    [TSSAvgSpeed] float  NULL,
    [WRTMSpeed] float  NULL,
    [CVAvgSpeed] float  NOT NULL,
    [NumberCVs] smallint  NOT NULL,
    [NumberQueuedCVs] smallint  NOT NULL,
    [PercentQueuedCVs] smallint  NULL,
    [CVQueuedState] bit  NULL,
    [CVCongestedState] bit  NULL,
    [RecommendedTargetSpeed] smallint  NULL,
    [RecommendedTargetSpeedSource] nvarchar(50)  NULL
);
GO

-- Creating table 'TME_ESSData_Input'
CREATE TABLE [dbo].[TME_ESSData_Input] (
    [ESSId] nvarchar(10)  NOT NULL,
    [ESSVisibilityDistance] float  NULL,
    [ESSVisibilityStatus] nvarchar(255)  NULL,
    [ESSCoefficientOfFriction] float  NULL,
    [ESSSurfaceStatus] nvarchar(255)  NULL,
    [DateReceived] datetime  NOT NULL
);
GO

-- Creating table 'TME_ESSMobileData_Input'
CREATE TABLE [dbo].[TME_ESSMobileData_Input] (
    [MobileESSId] nvarchar(10)  NOT NULL,
    [Latitude] float  NULL,
    [Longitude] float  NOT NULL,
    [Bearing] float  NULL,
    [Friction] float  NULL,
    [ObservationPavement] nvarchar(25)  NULL
);
GO

-- Creating table 'TME_TSSData_Input'
CREATE TABLE [dbo].[TME_TSSData_Input] (
    [DZId] nvarchar(25)  NOT NULL,
    [DSId] nvarchar(50)  NULL,
    [DateReceived] datetime  NULL,
    [StartInterval] smallint  NULL,
    [EndInterval] smallint  NULL,
    [IntervalLength] smallint  NULL,
    [BeginTime] datetime  NULL,
    [EndTime] datetime  NULL,
    [Volume] smallint  NOT NULL,
    [Occupancy] real  NOT NULL,
    [AvgSpeed] smallint  NOT NULL,
    [Queued] bit  NULL,
    [Congested] bit  NULL,
    [DZStatus] nvarchar(25)  NULL,
    [DataType] nvarchar(25)  NULL,
    [RoadwayId] nvarchar(25)  NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'TME_TSSESS_Link'
CREATE TABLE [dbo].[TME_TSSESS_Link] (
    [RoadwayId] nvarchar(25)  NULL,
    [LinkId] nvarchar(25)  NOT NULL,
    [DateProcessed] datetime  NOT NULL,
    [IntervalLength] smallint  NULL,
    [TSSSpeed] float  NOT NULL,
    [ESSSpeed] float  NULL,
    [TSSVolume] smallint  NOT NULL,
    [TSSOccupancy] float  NULL,
    [Congested] bit  NULL,
    [Queued] bit  NULL,
    [RecommendedSpeed] smallint  NULL,
    [RecommendedSpeedSource] nvarchar(50)  NULL,
    [CurrentDMSMessage] nvarchar(255)  NULL,
    [CurrentVSLSignSpeed] smallint  NULL
);
GO

-- Creating table 'TMEOutput_QWARN_QueueInfo'
CREATE TABLE [dbo].[TMEOutput_QWARN_QueueInfo] (
    [DateGenerated] datetime  NOT NULL,
    [BOQ_MMLocation] smallint  NOT NULL,
    [BOQ_LinkId] nvarchar(25)  NOT NULL,
    [FOQ_MMLocation] float  NOT NULL,
    [FOQ_LinkId] nvarchar(25)  NOT NULL,
    [RateOfQueueGrowth] float  NOT NULL,
    [SPeed_In_Queue] smallint  NOT NULL
);
GO

-- Creating table 'TMEOutput_QWARNMessage_CV'
CREATE TABLE [dbo].[TMEOutput_QWARNMessage_CV] (
    [DateGenerated] datetime  NOT NULL,
    [RoadwayID] nvarchar(25)  NULL,
    [BOQMMLocation] float  NOT NULL,
    [FOQMMLocation] float  NOT NULL,
    [SpeedInQueue] smallint  NULL,
    [RateOfQueueGrowth] float  NULL,
    [ValidityDuration] int  NULL
);
GO

-- Creating table 'TMEOutput_ShockWaveInformation'
CREATE TABLE [dbo].[TMEOutput_ShockWaveInformation] (
    [Timestamp] datetime  NOT NULL,
    [RoadwayIdentifier] nvarchar(25)  NOT NULL,
    [MileMarkerLocation] float  NOT NULL,
    [SchockwaveSpeed] smallint  NOT NULL,
    [SchockwaveDirection] nvarchar(15)  NULL
);
GO

-- Creating table 'TMEOutput_SPDHARMMessage_CV'
CREATE TABLE [dbo].[TMEOutput_SPDHARMMessage_CV] (
    [DateGenerated] datetime  NOT NULL,
    [RoadwayId] nvarchar(255)  NULL,
    [RecommendedSpeed] smallint  NOT NULL,
    [BeginMM] float  NOT NULL,
    [EndMM] float  NULL,
    [Justification] nvarchar(25)  NOT NULL,
    [ValidityDuration] int  NULL
);
GO

-- Creating table 'TMEOutput_SPDHARMMessage_Infrastructure'
CREATE TABLE [dbo].[TMEOutput_SPDHARMMessage_Infrastructure] (
    [DateGenerated] datetime  NOT NULL,
    [RecommendedSpeed] smallint  NOT NULL,
    [VSLId] nvarchar(25)  NOT NULL,
    [Justification] nvarchar(25)  NOT NULL,
    [LinkId] nvarchar(255)  NULL
);
GO

-- Creating table 'TMEOutput_WRTM_Alerts'
CREATE TABLE [dbo].[TMEOutput_WRTM_Alerts] (
    [DateGenerated] datetime  NOT NULL,
    [WeatherAlert] nvarchar(255)  NOT NULL,
    [Justification] nvarchar(25)  NOT NULL,
    [RoadwayId] nvarchar(25)  NOT NULL,
    [BeginMM] float  NOT NULL,
    [EndMM] float  NOT NULL,
    [BeginTime] datetime  NULL,
    [EndTime] datetime  NULL
);
GO

-- Creating table 'RoadWeatherProbeInputs'
CREATE TABLE [dbo].[RoadWeatherProbeInputs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NomadicDeviceId] nvarchar(50)  NOT NULL,
    [DateGenerated] datetime  NOT NULL,
    [DateCreated] datetime  NOT NULL,
    [AirTemperature] float  NULL,
    [AtmosphericPressure] float  NULL,
    [HeadlightStatus] bit  NULL,
    [Speed] float  NOT NULL,
    [SteeringWheelAngle] float  NULL,
    [WiperStatus] bit  NULL,
    [RightFrontWheelSpeed] float  NULL,
    [LeftFrontWheelSpeed] float  NULL,
    [LeftRearWheelSpeed] float  NULL,
    [RightRearWheelSpeed] float  NULL,
    [GpsHeading] float  NOT NULL,
    [GpsLatitude] float  NOT NULL,
    [GpsLongitude] float  NOT NULL,
    [GpsElevation] float  NOT NULL,
    [GpsSpeed] float  NOT NULL
);
GO

-- Creating table 'MAWOutputs'
CREATE TABLE [dbo].[MAWOutputs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AlertRequestTime] datetime  NOT NULL,
    [AlertTime] datetime  NOT NULL,
    [AlertGenTime] datetime  NOT NULL,
    [PrecipitationCode] int  NOT NULL,
    [PavementCode] int  NOT NULL,
    [VisibilityCode] int  NOT NULL,
    [ActionCode] int  NOT NULL
);
GO

-- Creating table 'WeatherEvents'
CREATE TABLE [dbo].[WeatherEvents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [StartTime] datetime  NOT NULL,
    [EndTime] datetime  NULL,
    [Mode] smallint  NOT NULL
);
GO

-- Creating table 'EDMSSAlerts'
CREATE TABLE [dbo].[EDMSSAlerts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL
);
GO

-- Creating table 'Districts'
CREATE TABLE [dbo].[Districts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EDMSSAlertId] int  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [Hr06AlertSummaryCode] nvarchar(100)  NOT NULL,
    [Hr24AlertSummaryCode] nvarchar(100)  NOT NULL,
    [Hr72AlertSummaryCode] nvarchar(100)  NOT NULL,
    [MaxLatitude] float  NOT NULL,
    [MaxLongitude] float  NOT NULL,
    [MinLatitude] float  NOT NULL,
    [MinLongitude] float  NOT NULL,
    [ObservAlertSummaryCode] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'Sites'
CREATE TABLE [dbo].[Sites] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Hr06AlertSummaryCode] nvarchar(100)  NOT NULL,
    [Hr24AlertSummaryCode] nvarchar(100)  NOT NULL,
    [Hr72AlertSummaryCode] nvarchar(100)  NOT NULL,
    [IsRoadCondSite] bit  NOT NULL,
    [IsWeatherObservSite] bit  NOT NULL,
    [Latitude] float  NOT NULL,
    [Longitude] float  NOT NULL,
    [ObservAlertCode] nvarchar(100)  NOT NULL,
    [SiteIdName] nvarchar(100)  NOT NULL,
    [SiteNum] bigint  NOT NULL,
    [DistrictId] int  NOT NULL,
    [MileMarker] float  NULL
);
GO

-- Creating table 'SiteObservations'
CREATE TABLE [dbo].[SiteObservations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DateTime] datetime  NOT NULL,
    [AlertCode] nvarchar(100)  NOT NULL,
    [Chemical] nvarchar(100)  NOT NULL,
    [Pavement] nvarchar(100)  NOT NULL,
    [Plow] nvarchar(100)  NOT NULL,
    [Precipitation] nvarchar(100)  NOT NULL,
    [RoadTemp] float  NOT NULL,
    [TreatmentAlertCode] nvarchar(100)  NOT NULL,
    [Visibility] nvarchar(100)  NOT NULL,
    [SiteId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ESSId] in table 'Configuration_ESS'
ALTER TABLE [dbo].[Configuration_ESS]
ADD CONSTRAINT [PK_Configuration_ESS]
    PRIMARY KEY CLUSTERED ([ESSId] ASC);
GO

-- Creating primary key on [Id] in table 'Configuration_INFLOThresholds'
ALTER TABLE [dbo].[Configuration_INFLOThresholds]
ADD CONSTRAINT [PK_Configuration_INFLOThresholds]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Name], [Direction], [BeginMM], [EndMM], [MMIncreasingDirection] in table 'Configuration_Roadway'
ALTER TABLE [dbo].[Configuration_Roadway]
ADD CONSTRAINT [PK_Configuration_Roadway]
    PRIMARY KEY CLUSTERED ([Name], [Direction], [BeginMM], [EndMM], [MMIncreasingDirection] ASC);
GO

-- Creating primary key on [RoadwayId], [ESSId] in table 'Configuration_RoadwayESS'
ALTER TABLE [dbo].[Configuration_RoadwayESS]
ADD CONSTRAINT [PK_Configuration_RoadwayESS]
    PRIMARY KEY CLUSTERED ([RoadwayId], [ESSId] ASC);
GO

-- Creating primary key on [LinkId], [BeginMM], [EndMM], [RoadwayId] in table 'Configuration_RoadwayLinks'
ALTER TABLE [dbo].[Configuration_RoadwayLinks]
ADD CONSTRAINT [PK_Configuration_RoadwayLinks]
    PRIMARY KEY CLUSTERED ([LinkId], [BeginMM], [EndMM], [RoadwayId] ASC);
GO

-- Creating primary key on [LinkId], [DSId] in table 'Configuration_RoadwayLinksDetectorStation'
ALTER TABLE [dbo].[Configuration_RoadwayLinksDetectorStation]
ADD CONSTRAINT [PK_Configuration_RoadwayLinksDetectorStation]
    PRIMARY KEY CLUSTERED ([LinkId], [DSId] ASC);
GO

-- Creating primary key on [LinkId], [ESSId] in table 'Configuration_RoadwayLinksESS'
ALTER TABLE [dbo].[Configuration_RoadwayLinksESS]
ADD CONSTRAINT [PK_Configuration_RoadwayLinksESS]
    PRIMARY KEY CLUSTERED ([LinkId], [ESSId] ASC);
GO

-- Creating primary key on [RoadwayId], [MMNumber], [Latitude1], [Longitude1], [Latitude2], [Longitude2] in table 'Configuration_RoadwayMileMarkers'
ALTER TABLE [dbo].[Configuration_RoadwayMileMarkers]
ADD CONSTRAINT [PK_Configuration_RoadwayMileMarkers]
    PRIMARY KEY CLUSTERED ([RoadwayId], [MMNumber], [Latitude1], [Longitude1], [Latitude2], [Longitude2] ASC);
GO

-- Creating primary key on [RoadwayId], [SubLinkId], [BeginMM], [EndMM] in table 'Configuration_RoadwaySubLinks'
ALTER TABLE [dbo].[Configuration_RoadwaySubLinks]
ADD CONSTRAINT [PK_Configuration_RoadwaySubLinks]
    PRIMARY KEY CLUSTERED ([RoadwayId], [SubLinkId], [BeginMM], [EndMM] ASC);
GO

-- Creating primary key on [DSId], [DZId], [LaneNumber] in table 'Configuration_TSSDetectionZone'
ALTER TABLE [dbo].[Configuration_TSSDetectionZone]
ADD CONSTRAINT [PK_Configuration_TSSDetectionZone]
    PRIMARY KEY CLUSTERED ([DSId], [DZId], [LaneNumber] ASC);
GO

-- Creating primary key on [DSId], [MMLocation], [NumberDetectionZones] in table 'Configuration_TSSDetectorStation'
ALTER TABLE [dbo].[Configuration_TSSDetectorStation]
ADD CONSTRAINT [PK_Configuration_TSSDetectorStation]
    PRIMARY KEY CLUSTERED ([DSId], [MMLocation], [NumberDetectionZones] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [CVMessageIdentifier] in table 'TME_CVData_Input'
ALTER TABLE [dbo].[TME_CVData_Input]
ADD CONSTRAINT [PK_TME_CVData_Input]
    PRIMARY KEY CLUSTERED ([CVMessageIdentifier] ASC);
GO

-- Creating primary key on [MMLocation] in table 'TME_CVData_Input_Processed'
ALTER TABLE [dbo].[TME_CVData_Input_Processed]
ADD CONSTRAINT [PK_TME_CVData_Input_Processed]
    PRIMARY KEY CLUSTERED ([MMLocation] ASC);
GO

-- Creating primary key on [SubLinkId], [DateProcessed], [CVAvgSpeed], [NumberCVs], [NumberQueuedCVs] in table 'TME_CVData_SubLink'
ALTER TABLE [dbo].[TME_CVData_SubLink]
ADD CONSTRAINT [PK_TME_CVData_SubLink]
    PRIMARY KEY CLUSTERED ([SubLinkId], [DateProcessed], [CVAvgSpeed], [NumberCVs], [NumberQueuedCVs] ASC);
GO

-- Creating primary key on [ESSId], [DateReceived] in table 'TME_ESSData_Input'
ALTER TABLE [dbo].[TME_ESSData_Input]
ADD CONSTRAINT [PK_TME_ESSData_Input]
    PRIMARY KEY CLUSTERED ([ESSId], [DateReceived] ASC);
GO

-- Creating primary key on [MobileESSId], [Longitude] in table 'TME_ESSMobileData_Input'
ALTER TABLE [dbo].[TME_ESSMobileData_Input]
ADD CONSTRAINT [PK_TME_ESSMobileData_Input]
    PRIMARY KEY CLUSTERED ([MobileESSId], [Longitude] ASC);
GO

-- Creating primary key on [Id] in table 'TME_TSSData_Input'
ALTER TABLE [dbo].[TME_TSSData_Input]
ADD CONSTRAINT [PK_TME_TSSData_Input]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [LinkId], [DateProcessed], [TSSSpeed], [TSSVolume] in table 'TME_TSSESS_Link'
ALTER TABLE [dbo].[TME_TSSESS_Link]
ADD CONSTRAINT [PK_TME_TSSESS_Link]
    PRIMARY KEY CLUSTERED ([LinkId], [DateProcessed], [TSSSpeed], [TSSVolume] ASC);
GO

-- Creating primary key on [DateGenerated], [BOQ_MMLocation], [BOQ_LinkId], [FOQ_MMLocation], [FOQ_LinkId], [RateOfQueueGrowth], [SPeed_In_Queue] in table 'TMEOutput_QWARN_QueueInfo'
ALTER TABLE [dbo].[TMEOutput_QWARN_QueueInfo]
ADD CONSTRAINT [PK_TMEOutput_QWARN_QueueInfo]
    PRIMARY KEY CLUSTERED ([DateGenerated], [BOQ_MMLocation], [BOQ_LinkId], [FOQ_MMLocation], [FOQ_LinkId], [RateOfQueueGrowth], [SPeed_In_Queue] ASC);
GO

-- Creating primary key on [DateGenerated], [BOQMMLocation], [FOQMMLocation] in table 'TMEOutput_QWARNMessage_CV'
ALTER TABLE [dbo].[TMEOutput_QWARNMessage_CV]
ADD CONSTRAINT [PK_TMEOutput_QWARNMessage_CV]
    PRIMARY KEY CLUSTERED ([DateGenerated], [BOQMMLocation], [FOQMMLocation] ASC);
GO

-- Creating primary key on [Timestamp], [RoadwayIdentifier], [MileMarkerLocation], [SchockwaveSpeed] in table 'TMEOutput_ShockWaveInformation'
ALTER TABLE [dbo].[TMEOutput_ShockWaveInformation]
ADD CONSTRAINT [PK_TMEOutput_ShockWaveInformation]
    PRIMARY KEY CLUSTERED ([Timestamp], [RoadwayIdentifier], [MileMarkerLocation], [SchockwaveSpeed] ASC);
GO

-- Creating primary key on [DateGenerated], [RecommendedSpeed], [BeginMM], [Justification] in table 'TMEOutput_SPDHARMMessage_CV'
ALTER TABLE [dbo].[TMEOutput_SPDHARMMessage_CV]
ADD CONSTRAINT [PK_TMEOutput_SPDHARMMessage_CV]
    PRIMARY KEY CLUSTERED ([DateGenerated], [RecommendedSpeed], [BeginMM], [Justification] ASC);
GO

-- Creating primary key on [DateGenerated], [RecommendedSpeed], [VSLId], [Justification] in table 'TMEOutput_SPDHARMMessage_Infrastructure'
ALTER TABLE [dbo].[TMEOutput_SPDHARMMessage_Infrastructure]
ADD CONSTRAINT [PK_TMEOutput_SPDHARMMessage_Infrastructure]
    PRIMARY KEY CLUSTERED ([DateGenerated], [RecommendedSpeed], [VSLId], [Justification] ASC);
GO

-- Creating primary key on [DateGenerated], [WeatherAlert], [Justification], [RoadwayId], [BeginMM], [EndMM] in table 'TMEOutput_WRTM_Alerts'
ALTER TABLE [dbo].[TMEOutput_WRTM_Alerts]
ADD CONSTRAINT [PK_TMEOutput_WRTM_Alerts]
    PRIMARY KEY CLUSTERED ([DateGenerated], [WeatherAlert], [Justification], [RoadwayId], [BeginMM], [EndMM] ASC);
GO

-- Creating primary key on [Id] in table 'RoadWeatherProbeInputs'
ALTER TABLE [dbo].[RoadWeatherProbeInputs]
ADD CONSTRAINT [PK_RoadWeatherProbeInputs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MAWOutputs'
ALTER TABLE [dbo].[MAWOutputs]
ADD CONSTRAINT [PK_MAWOutputs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WeatherEvents'
ALTER TABLE [dbo].[WeatherEvents]
ADD CONSTRAINT [PK_WeatherEvents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EDMSSAlerts'
ALTER TABLE [dbo].[EDMSSAlerts]
ADD CONSTRAINT [PK_EDMSSAlerts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Districts'
ALTER TABLE [dbo].[Districts]
ADD CONSTRAINT [PK_Districts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Sites'
ALTER TABLE [dbo].[Sites]
ADD CONSTRAINT [PK_Sites]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SiteObservations'
ALTER TABLE [dbo].[SiteObservations]
ADD CONSTRAINT [PK_SiteObservations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [EDMSSAlertId] in table 'Districts'
ALTER TABLE [dbo].[Districts]
ADD CONSTRAINT [FK_EDMSSAlertDistrict]
    FOREIGN KEY ([EDMSSAlertId])
    REFERENCES [dbo].[EDMSSAlerts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EDMSSAlertDistrict'
CREATE INDEX [IX_FK_EDMSSAlertDistrict]
ON [dbo].[Districts]
    ([EDMSSAlertId]);
GO

-- Creating foreign key on [DistrictId] in table 'Sites'
ALTER TABLE [dbo].[Sites]
ADD CONSTRAINT [FK_DistrictSite]
    FOREIGN KEY ([DistrictId])
    REFERENCES [dbo].[Districts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DistrictSite'
CREATE INDEX [IX_FK_DistrictSite]
ON [dbo].[Sites]
    ([DistrictId]);
GO

-- Creating foreign key on [SiteId] in table 'SiteObservations'
ALTER TABLE [dbo].[SiteObservations]
ADD CONSTRAINT [FK_SiteSiteObservation]
    FOREIGN KEY ([SiteId])
    REFERENCES [dbo].[Sites]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SiteSiteObservation'
CREATE INDEX [IX_FK_SiteSiteObservation]
ON [dbo].[SiteObservations]
    ([SiteId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------