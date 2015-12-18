using System;


using SharpRepository;
using SharpRepository.InMemoryRepository;
using SharpRepository.Repository;
using InfloCommon;

namespace InfloCommon.Repositories
{
    //Depricated after we ripped out SharpRespository. Need resurrection if we want to use this.
    public class InMemoryUnitOfWork //: IUnitOfWork
    {
 

        public IRepository<Configuration_ESS> Configuration_ESSs
        {
            get { return _Configuration_ESS ?? (_Configuration_ESS = new InMemoryRepository<Configuration_ESS>()); }
        }

        public IRepository<Configuration_INFLOThresholds> Configuration_InfloThresholds
        {
            get { return _Configuration_INFLOThresholds ?? (_Configuration_INFLOThresholds = new InMemoryRepository<Configuration_INFLOThresholds>()); }
        }

        public IRepository<Configuration_Roadway> Configuration_Roadways
        {
            get { return _Configuration_Roadway ?? (_Configuration_Roadway = new InMemoryRepository<Configuration_Roadway>()); }
        }

        public IRepository<Configuration_RoadwayESS> Configuration_RoadwayESSs
        {
            get { return _Configuration_RoadwayESS ?? (_Configuration_RoadwayESS = new InMemoryRepository<Configuration_RoadwayESS>()); }
        }

        public IRepository<Configuration_RoadwayLinks> Configuration_RoadwayLinks
        {
            get { return _Configuration_RoadwayLinks ?? (_Configuration_RoadwayLinks = new InMemoryRepository<Configuration_RoadwayLinks>()); }
        }

        public IRepository<Configuration_RoadwayLinksDetectorStation> Configuration_RoadwayLinksDetectorStations
        {
            get { return _Configuration_RoadwayLinksDetectorStation ?? (_Configuration_RoadwayLinksDetectorStation = new InMemoryRepository<Configuration_RoadwayLinksDetectorStation>()); }
        }

        public IRepository<Configuration_RoadwayLinksESS> Configuration_RoadwayLinksESSs
        {
            get { return _Configuration_RoadwayLinksESS ?? (_Configuration_RoadwayLinksESS = new InMemoryRepository<Configuration_RoadwayLinksESS>()); }
        }

        public IRepository<Configuration_RoadwayMileMarkers> Configuration_RoadwayMileMarkers
        {
            get { return _Configuration_RoadwayMileMarkers ?? (_Configuration_RoadwayMileMarkers = new InMemoryRepository<Configuration_RoadwayMileMarkers>()); }
        }

        public IRepository<Configuration_RoadwaySubLinks> Configuration_RoadwaySubLinks
        {
            get { return _Configuration_RoadwaySubLinks ?? (_Configuration_RoadwaySubLinks = new InMemoryRepository<Configuration_RoadwaySubLinks>()); }
        }

        public IRepository<Configuration_TSSDetectionZone> Configuration_TSSDetectionZones
        {
            get { return _Configuration_TSSDetectionZone ?? (_Configuration_TSSDetectionZone = new InMemoryRepository<Configuration_TSSDetectionZone>()); }
        }


        public IRepository<Configuration_TSSDetectorStation> Configuration_TSSDetectorStations
        {
            get { return _Configuration_TSSDetectorStation ?? (_Configuration_TSSDetectorStation = new InMemoryRepository<Configuration_TSSDetectorStation>()); }
        }

        public IRepository<TME_CVData_Input> TME_CVData_Inputs
        {
            get { return _TME_CVData_Input ?? (_TME_CVData_Input = new InMemoryRepository<TME_CVData_Input>()); }
        }

        public IRepository<TME_CVData_Input_Processed> TME_CVData_Input_Processeds
        {
            get { return _TME_CVData_Input_Processed ?? (_TME_CVData_Input_Processed = new InMemoryRepository<TME_CVData_Input_Processed>()); }
        }

        public IRepository<TME_CVData_SubLink> TME_CVData_SubLinks
        {
            get { return _TME_CVData_SubLink ?? (_TME_CVData_SubLink = new InMemoryRepository<TME_CVData_SubLink>()); }
        }

        public IRepository<TME_ESSData_Input> TME_ESSData_Inputs
        {
            get { return _TME_ESSData_Input ?? (_TME_ESSData_Input = new InMemoryRepository<TME_ESSData_Input>()); }
        }

        public IRepository<TME_ESSMobileData_Input> TME_ESSMobileData_Inputs
        {
            get { return _TME_ESSMobileData_Input ?? (_TME_ESSMobileData_Input = new InMemoryRepository<TME_ESSMobileData_Input>()); }
        }

        public IRepository<TME_TSSData_Input> TME_TSSData_Inputs
        {
            get { return _TME_TSSData_Input ?? (_TME_TSSData_Input = new InMemoryRepository<TME_TSSData_Input>()); }
        }

        public IRepository<TME_TSSESS_Link> TME_TSSESS_Links
        {
            get { return _TME_TSSESS_Link ?? (_TME_TSSESS_Link = new InMemoryRepository<TME_TSSESS_Link>()); }
        }

        public IRepository<TMEOutput_QWARN_QueueInfo> TMEOutput_QWARN_QueueInfos
        {
            get { return _TMEOutput_QWARN_QueueInfo ?? (_TMEOutput_QWARN_QueueInfo = new InMemoryRepository<TMEOutput_QWARN_QueueInfo>()); }
        }

        public IRepository<TMEOutput_QWARNMessage_CV> TMEOutput_QWARNMessage_CVs
        {
            get { return _TMEOutput_QWARNMessage_CV ?? (_TMEOutput_QWARNMessage_CV = new InMemoryRepository<TMEOutput_QWARNMessage_CV>()); }
        }

        public IRepository<TMEOutput_ShockWaveInformation> TMEOutput_ShockWaveInformations
        {
            get { return _TMEOutput_ShockWaveInformation ?? (_TMEOutput_ShockWaveInformation = new InMemoryRepository<TMEOutput_ShockWaveInformation>()); }
        }

        public IRepository<TMEOutput_SPDHARMMessage_CV> TMEOutput_SPDHARMMessage_CVs
        {
            get { return _TMEOutput_SPDHARMMessage_CV ?? (_TMEOutput_SPDHARMMessage_CV = new InMemoryRepository<TMEOutput_SPDHARMMessage_CV>()); }
        }

        public IRepository<TMEOutput_SPDHARMMessage_Infrastructure> TMEOutput_SPDHARMMessage_Infrastructures
        {
            get { return _TMEOutput_SPDHARMMessage_Infrastructure ?? (_TMEOutput_SPDHARMMessage_Infrastructure = new InMemoryRepository<TMEOutput_SPDHARMMessage_Infrastructure>()); }
        }

        public IRepository<TMEOutput_WRTM_Alerts> TMEOutput_WRTM_Alerts
        {
            get { return _TMEOutput_WRTM_Alerts ?? (_TMEOutput_WRTM_Alerts = new InMemoryRepository<TMEOutput_WRTM_Alerts>()); }
        }


        public IRepository<RoadWeatherProbeInputs> RoadWeatherProbeInputs
        {
            get { return _RoadWeatherProbeInputs ?? (_RoadWeatherProbeInputs = new InMemoryRepository<RoadWeatherProbeInputs>()); }
        }
        public IRepository<EDMSSAlert> EDMSSAlerts
        {
            get { return _EDMSSAlerts ?? (_EDMSSAlerts = new InMemoryRepository<EDMSSAlert>()); }
        }
        public IRepository<District> Districts
        {
            get { return _Districts ?? (_Districts = new InMemoryRepository<District>()); }
        }
        public IRepository<Site> Sites
        {
            get { return _Sites ?? (_Sites = new InMemoryRepository<Site>()); }
        }
        public IRepository<SiteObservation> SiteObservations
        {
            get { return _SiteObservations ?? (_SiteObservations = new InMemoryRepository<SiteObservation>()); }
        }
        public void Commit()
        {
            Committed = true;
        }
        public void SaveChanges()
        {
            Committed = true;
        }
        public System.Data.Entity.Infrastructure.DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
            throw new NotImplementedException();//Dont' know how to implement this for inMemory
            //return _context.Entry(entity);
        }
        public bool Committed { get; set; }

      

        InMemoryRepository<Configuration_ESS> _Configuration_ESS;
        InMemoryRepository<Configuration_INFLOThresholds> _Configuration_INFLOThresholds;
        InMemoryRepository<Configuration_Roadway> _Configuration_Roadway;
        InMemoryRepository<Configuration_RoadwayESS> _Configuration_RoadwayESS;
        InMemoryRepository<Configuration_RoadwayLinks> _Configuration_RoadwayLinks;
        InMemoryRepository<Configuration_RoadwayLinksDetectorStation> _Configuration_RoadwayLinksDetectorStation;
        InMemoryRepository<Configuration_RoadwayLinksESS> _Configuration_RoadwayLinksESS;
        InMemoryRepository<Configuration_RoadwayMileMarkers> _Configuration_RoadwayMileMarkers;
        InMemoryRepository<Configuration_RoadwaySubLinks> _Configuration_RoadwaySubLinks;
        InMemoryRepository<Configuration_TSSDetectionZone> _Configuration_TSSDetectionZone;
        InMemoryRepository<Configuration_TSSDetectorStation> _Configuration_TSSDetectorStation;
        InMemoryRepository<TME_CVData_Input> _TME_CVData_Input;
        InMemoryRepository<TME_CVData_Input_Processed> _TME_CVData_Input_Processed;
        InMemoryRepository<TME_CVData_SubLink> _TME_CVData_SubLink;
        InMemoryRepository<TME_ESSData_Input> _TME_ESSData_Input;
        InMemoryRepository<TME_ESSMobileData_Input> _TME_ESSMobileData_Input;
        InMemoryRepository<TME_TSSData_Input> _TME_TSSData_Input;
        InMemoryRepository<TME_TSSESS_Link> _TME_TSSESS_Link;
        InMemoryRepository<TMEOutput_QWARN_QueueInfo> _TMEOutput_QWARN_QueueInfo;
        InMemoryRepository<TMEOutput_QWARNMessage_CV> _TMEOutput_QWARNMessage_CV;
        InMemoryRepository<TMEOutput_ShockWaveInformation> _TMEOutput_ShockWaveInformation;
        InMemoryRepository<TMEOutput_SPDHARMMessage_CV> _TMEOutput_SPDHARMMessage_CV;
        InMemoryRepository<TMEOutput_SPDHARMMessage_Infrastructure> _TMEOutput_SPDHARMMessage_Infrastructure;
        InMemoryRepository<TMEOutput_WRTM_Alerts> _TMEOutput_WRTM_Alerts;

        InMemoryRepository<RoadWeatherProbeInputs> _RoadWeatherProbeInputs;
        InMemoryRepository<EDMSSAlert> _EDMSSAlerts;
        InMemoryRepository<District> _Districts;
        InMemoryRepository<Site> _Sites;
        InMemoryRepository<SiteObservation> _SiteObservations;
    }
}
