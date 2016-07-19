
using InfloCommon;
using SharpRepository;
using SharpRepository.InMemoryRepository;
using SharpRepository.Repository;
using System;
using System.Data.Entity;


namespace InfloCommon.Repositories
{
    public interface IUnitOfWork : IDisposable
    {

        DbSet<Configuration_ESS> Configuration_ESSs { get; set; }
        DbSet<Configuration_INFLOThresholds> Configuration_INFLOThresholds { get; set; }
        DbSet<Configuration_Roadway> Configuration_Roadways { get; set; }
        DbSet<Configuration_RoadwayESS> Configuration_RoadwayESSs { get; set; }
        DbSet<Configuration_RoadwayLinks> Configuration_RoadwayLinks { get; set; }
        DbSet<Configuration_RoadwayLinksDetectorStation> Configuration_RoadwayLinksDetectorStations { get; set; }
        DbSet<Configuration_RoadwayLinksESS> Configuration_RoadwayLinksESSs { get; set; }
        DbSet<Configuration_RoadwayMileMarkers> Configuration_RoadwayMileMarkers { get; set; }
        DbSet<Configuration_RoadwaySubLinks> Configuration_RoadwaySubLinks { get; set; }
        DbSet<Configuration_TSSDetectionZone> Configuration_TSSDetectionZones { get; set; }
        DbSet<Configuration_TSSDetectorStation> Configuration_TSSDetectorStations { get; set; }
        DbSet<TME_CVData_Input> TME_CVData_Inputs { get; set; }
        DbSet<TME_CVData_Input_Processed> TME_CVData_Input_Processeds { get; set; }
        DbSet<TME_CVData_SubLink> TME_CVData_SubLinks { get; set; }
        DbSet<TME_ESSData_Input> TME_ESSData_Inputs { get; set; }
        DbSet<TME_ESSMobileData_Input> TME_ESSMobileData_Inputs { get; set; }
        DbSet<TME_TSSData_Input> TME_TSSData_Inputs { get; set; }
        DbSet<TME_TSSESS_Link> TME_TSSESS_Links { get; set; }
        DbSet<TMEOutput_QWARN_QueueInfo> TMEOutput_QWARN_QueueInfos { get; set; }
        DbSet<TMEOutput_QWARNMessage_CV> TMEOutput_QWARNMessage_CVs { get; set; }
        DbSet<TMEOutput_ShockWaveInformation> TMEOutput_ShockWaveInformations { get; set; }
        DbSet<TMEOutput_SPDHARMMessage_CV> TMEOutput_SPDHARMMessage_CVs { get; set; }
        DbSet<TMEOutput_SPDHARMMessage_Infrastructure> TMEOutput_SPDHARMMessage_Infrastructures { get; set; }
        DbSet<TMEOutput_WRTM_Alerts> TMEOutput_WRTM_Alerts { get; set; }
        DbSet<MAWOutput> MAWOutputs { get; set; }
        DbSet<RoadWeatherProbeInputs> RoadWeatherProbeInputs { get; set; }
        DbSet<EDMSSAlert> EDMSSAlerts { get; set; }
        DbSet<District> Districts { get; set; }
        DbSet<Site> Sites { get; set; }
        DbSet<SiteObservation> SiteObservations { get; set; }
        DbSet<WeatherEvent> WeatherEvents { get; set; }
        DbSet<WeatherLog> WeatherLogs { get; set; }

      //  IRepository<Configuration_ESS> Configuration_ESSs { get; }

      //  IRepository<Configuration_INFLOThresholds> Configuration_InfloThresholds { get; }

      //  IRepository<Configuration_Roadway> Configuration_Roadways { get; }

      //  IRepository<Configuration_RoadwayESS> Configuration_RoadwayESSs { get; }

      //  IRepository<Configuration_RoadwayLinks> Configuration_RoadwayLinks { get; }

      //  IRepository<Configuration_RoadwayLinksDetectorStation> Configuration_RoadwayLinksDetectorStations { get; }

      //  IRepository<Configuration_RoadwayLinksESS> Configuration_RoadwayLinksESSs { get; }
      //  IRepository<Configuration_RoadwayMileMarkers> Configuration_RoadwayMileMarkers { get; }

      //  IRepository<Configuration_RoadwaySubLinks> Configuration_RoadwaySubLinks { get; }
      //  IRepository<Configuration_TSSDetectionZone> Configuration_TSSDetectionZones { get; }
      //  IRepository<Configuration_TSSDetectorStation> Configuration_TSSDetectorStations { get; }

      //  IRepository<TME_CVData_Input> TME_CVData_Inputs { get; }

      //  IRepository<TME_CVData_Input_Processed> TME_CVData_Input_Processeds { get; }

      //  IRepository<TME_CVData_SubLink> TME_CVData_SubLinks { get; }

      //  IRepository<TME_ESSData_Input> TME_ESSData_Inputs { get; }

      //  IRepository<TME_ESSMobileData_Input> TME_ESSMobileData_Inputs { get; }

      //  IRepository<TME_TSSData_Input> TME_TSSData_Inputs { get; }

      //  IRepository<TME_TSSESS_Link> TME_TSSESS_Links { get; }

      //  IRepository<TMEOutput_QWARN_QueueInfo> TMEOutput_QWARN_QueueInfos { get; }

      //  IRepository<TMEOutput_QWARNMessage_CV> TMEOutput_QWARNMessage_CVs { get; }

      //  IRepository<TMEOutput_ShockWaveInformation> TMEOutput_ShockWaveInformations { get; }

      //  IRepository<TMEOutput_SPDHARMMessage_CV> TMEOutput_SPDHARMMessage_CVs { get; }

      //  IRepository<TMEOutput_SPDHARMMessage_Infrastructure> TMEOutput_SPDHARMMessage_Infrastructures { get; }

      //  IRepository<TMEOutput_WRTM_Alerts> TMEOutput_WRTM_Alerts { get; }


      //IRepository<RoadWeatherProbeInputs> RoadWeatherProbeInputs { get; }
      //  IRepository<EDMSSAlert> EDMSSAlerts { get; }
      //  IRepository<District> Districts { get; }
      //  IRepository<Site> Sites { get; }
      //  IRepository<SiteObservation> SiteObservations { get; }
  
      

        int Commit();//for backward compat.
        int SaveChanges();

        System.Data.Entity.Infrastructure.DbEntityEntry<T> Entry<T>(T entity) where T:class;
    }
}


