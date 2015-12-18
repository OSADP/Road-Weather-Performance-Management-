using System.Data.Entity.Core.Objects;

//using SharpRepository;
//using SharpRepository.InMemoryRepository;
//using SharpRepository.Repository;
//using SharpRepository.EfRepository;
using System.Data.Entity;
using InfloCommon;
using System;


namespace InfloCommon.Repositories
{
    /// <summary>
    /// doubtful this really follows the 'unit of work' pattern anymore - SharpRepository ripped out due to 
    /// calling .Add() took 300 - 600 ms each call.  Now we are using dbcontext directly, much faster.
    /// </summary>
    public class UnitOfWork : DbContext, IUnitOfWork, IDisposable
    {
        public UnitOfWork(string connectionString)
            : base(connectionString)
        {
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }



        public virtual DbSet<Configuration_ESS> Configuration_ESSs { get; set; }
        public virtual DbSet<Configuration_INFLOThresholds> Configuration_INFLOThresholds { get; set; }
        public virtual DbSet<Configuration_Roadway> Configuration_Roadways { get; set; }
        public virtual DbSet<Configuration_RoadwayESS> Configuration_RoadwayESSs { get; set; }
        public virtual DbSet<Configuration_RoadwayLinks> Configuration_RoadwayLinks { get; set; }
        public virtual DbSet<Configuration_RoadwayLinksDetectorStation> Configuration_RoadwayLinksDetectorStations { get; set; }
        public virtual DbSet<Configuration_RoadwayLinksESS> Configuration_RoadwayLinksESSs { get; set; }
        public virtual DbSet<Configuration_RoadwayMileMarkers> Configuration_RoadwayMileMarkers { get; set; }
        public virtual DbSet<Configuration_RoadwaySubLinks> Configuration_RoadwaySubLinks { get; set; }
        public virtual DbSet<Configuration_TSSDetectionZone> Configuration_TSSDetectionZones { get; set; }
        public virtual DbSet<Configuration_TSSDetectorStation> Configuration_TSSDetectorStations { get; set; }
        public virtual DbSet<TME_CVData_Input> TME_CVData_Inputs { get; set; }
        public virtual DbSet<TME_CVData_Input_Processed> TME_CVData_Input_Processeds { get; set; }
        public virtual DbSet<TME_CVData_SubLink> TME_CVData_SubLinks { get; set; }
        public virtual DbSet<TME_ESSData_Input> TME_ESSData_Inputs { get; set; }
        public virtual DbSet<TME_ESSMobileData_Input> TME_ESSMobileData_Inputs { get; set; }
        public virtual DbSet<TME_TSSData_Input> TME_TSSData_Inputs { get; set; }
        public virtual DbSet<TME_TSSESS_Link> TME_TSSESS_Links { get; set; }
        public virtual DbSet<TMEOutput_QWARN_QueueInfo> TMEOutput_QWARN_QueueInfos { get; set; }
        public virtual DbSet<TMEOutput_QWARNMessage_CV> TMEOutput_QWARNMessage_CVs { get; set; }
        public virtual DbSet<TMEOutput_ShockWaveInformation> TMEOutput_ShockWaveInformations { get; set; }
        public virtual DbSet<TMEOutput_SPDHARMMessage_CV> TMEOutput_SPDHARMMessage_CVs { get; set; }
        public virtual DbSet<TMEOutput_SPDHARMMessage_Infrastructure> TMEOutput_SPDHARMMessage_Infrastructures { get; set; }
        public virtual DbSet<TMEOutput_WRTM_Alerts> TMEOutput_WRTM_Alerts { get; set; }

        public virtual DbSet<RoadWeatherProbeInputs> RoadWeatherProbeInputs { get; set; }
        public virtual DbSet<EDMSSAlert> EDMSSAlerts { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<SiteObservation> SiteObservations { get; set; }
        public virtual DbSet<WeatherEvent> WeatherEvents { get; set; }

        public int Commit()
        {
            return this.SaveChanges();
        }
        void IDisposable.Dispose()
        {
         
            base.Dispose(true);
        }


      //  public UnitOfWork(string connectionString)
      //  {
      //      _context = new DbContext(connectionString);
      //      _context.Configuration.LazyLoadingEnabled = true;


      //      RoadWeatherProbeInputs.DisableCaching();
      //  }

      //  public UnitOfWork(DbContext context)
      //  {
      //      _context = context;
      //      _context.Configuration.LazyLoadingEnabled = true;
      //  }


      //  public IRepository<Configuration_ESS> Configuration_ESSs
      //  {
      //      get { return _Configuration_ESS ?? (_Configuration_ESS = new EfRepository<Configuration_ESS>(_context)); }
      //  }

      //  public IRepository<Configuration_INFLOThresholds> Configuration_InfloThresholds
      //  {
      //      get { return _Configuration_INFLOThresholds ?? (_Configuration_INFLOThresholds = new EfRepository<Configuration_INFLOThresholds>(_context)); }
      //  }

      //  public IRepository<Configuration_Roadway> Configuration_Roadways
      //  {
      //      get { return _Configuration_Roadway ?? (_Configuration_Roadway = new EfRepository<Configuration_Roadway>(_context)); }
      //  }

      //  public IRepository<Configuration_RoadwayESS> Configuration_RoadwayESSs
      //  {
      //      get { return _Configuration_RoadwayESS ?? (_Configuration_RoadwayESS = new EfRepository<Configuration_RoadwayESS>(_context)); }
      //  }

      //  public IRepository<Configuration_RoadwayLinks> Configuration_RoadwayLinks
      //  {
      //      get { return _Configuration_RoadwayLinks ?? (_Configuration_RoadwayLinks = new EfRepository<Configuration_RoadwayLinks>(_context)); }
      //  }

      //  public IRepository<Configuration_RoadwayLinksDetectorStation> Configuration_RoadwayLinksDetectorStations
      //  {
      //      get { return _Configuration_RoadwayLinksDetectorStation ?? (_Configuration_RoadwayLinksDetectorStation = new EfRepository<Configuration_RoadwayLinksDetectorStation>(_context)); }
      //  }

      //  public IRepository<Configuration_RoadwayLinksESS> Configuration_RoadwayLinksESSs
      //  {
      //      get { return _Configuration_RoadwayLinksESS ?? (_Configuration_RoadwayLinksESS = new EfRepository<Configuration_RoadwayLinksESS>(_context)); }
      //  }

      //  public IRepository<Configuration_RoadwayMileMarkers> Configuration_RoadwayMileMarkers
      //  {
      //      get { return _Configuration_RoadwayMileMarkers ?? (_Configuration_RoadwayMileMarkers = new EfRepository<Configuration_RoadwayMileMarkers>(_context)); }
      //  }

      //  public IRepository<Configuration_RoadwaySubLinks> Configuration_RoadwaySubLinks
      //  {
      //      get { return _Configuration_RoadwaySubLinks ?? (_Configuration_RoadwaySubLinks = new EfRepository<Configuration_RoadwaySubLinks>(_context)); }
      //  }

      //  public IRepository<Configuration_TSSDetectionZone> Configuration_TSSDetectionZones
      //  {
      //      get { return _Configuration_TSSDetectionZone ?? (_Configuration_TSSDetectionZone = new EfRepository<Configuration_TSSDetectionZone>(_context)); }
      //  }

      //  public IRepository<Configuration_TSSDetectorStation> Configuration_TSSDetectorStations
      //  {
      //      get { return _Configuration_TSSDetectorStation ?? (_Configuration_TSSDetectorStation = new EfRepository<Configuration_TSSDetectorStation>(_context)); }
      //  }

      //  public IRepository<TME_CVData_Input> TME_CVData_Inputs
      //  {
      //      get { return _TME_CVData_Input ?? (_TME_CVData_Input = new EfRepository<TME_CVData_Input>(_context)); }
      //  }

      //  public IRepository<TME_CVData_Input_Processed> TME_CVData_Input_Processeds
      //  {
      //      get { return _TME_CVData_Input_Processed ?? (_TME_CVData_Input_Processed = new EfRepository<TME_CVData_Input_Processed>(_context)); }
      //  }

      //  public IRepository<TME_CVData_SubLink> TME_CVData_SubLinks
      //  {
      //      get { return _TME_CVData_SubLink ?? (_TME_CVData_SubLink = new EfRepository<TME_CVData_SubLink>(_context)); }
      //  }

      //  public IRepository<TME_ESSData_Input> TME_ESSData_Inputs
      //  {
      //      get { return _TME_ESSData_Input ?? (_TME_ESSData_Input = new EfRepository<TME_ESSData_Input>(_context)); }
      //  }

      //  public IRepository<TME_ESSMobileData_Input> TME_ESSMobileData_Inputs
      //  {
      //      get { return _TME_ESSMobileData_Input ?? (_TME_ESSMobileData_Input = new EfRepository<TME_ESSMobileData_Input>(_context)); }
      //  }

      //  public IRepository<TME_TSSData_Input> TME_TSSData_Inputs
      //  {
      //      get { return _TME_TSSData_Input ?? (_TME_TSSData_Input = new EfRepository<TME_TSSData_Input>(_context)); }
      //  }

      //  public IRepository<TME_TSSESS_Link> TME_TSSESS_Links
      //  {
      //      get { return _TME_TSSESS_Link ?? (_TME_TSSESS_Link = new EfRepository<TME_TSSESS_Link>(_context)); }
      //  }

      //  public IRepository<TMEOutput_QWARN_QueueInfo> TMEOutput_QWARN_QueueInfos
      //  {
      //      get { return _TMEOutput_QWARN_QueueInfo ?? (_TMEOutput_QWARN_QueueInfo = new EfRepository<TMEOutput_QWARN_QueueInfo>(_context)); }
      //  }

      //  public IRepository<TMEOutput_QWARNMessage_CV> TMEOutput_QWARNMessage_CVs
      //  {
      //      get { return _TMEOutput_QWARNMessage_CV ?? (_TMEOutput_QWARNMessage_CV = new EfRepository<TMEOutput_QWARNMessage_CV>(_context)); }
      //  }

      //  public IRepository<TMEOutput_ShockWaveInformation> TMEOutput_ShockWaveInformations
      //  {
      //      get { return _TMEOutput_ShockWaveInformation ?? (_TMEOutput_ShockWaveInformation = new EfRepository<TMEOutput_ShockWaveInformation>(_context)); }
      //  }

      //  public IRepository<TMEOutput_SPDHARMMessage_CV> TMEOutput_SPDHARMMessage_CVs
      //  {
      //      get { return _TMEOutput_SPDHARMMessage_CV ?? (_TMEOutput_SPDHARMMessage_CV = new EfRepository<TMEOutput_SPDHARMMessage_CV>(_context)); }
      //  }

      //  public IRepository<TMEOutput_SPDHARMMessage_Infrastructure> TMEOutput_SPDHARMMessage_Infrastructures
      //  {
      //      get { return _TMEOutput_SPDHARMMessage_Infrastructure ?? (_TMEOutput_SPDHARMMessage_Infrastructure = new EfRepository<TMEOutput_SPDHARMMessage_Infrastructure>(_context)); }
      //  }

      //  public IRepository<TMEOutput_WRTM_Alerts> TMEOutput_WRTM_Alerts
      //  {
      //      get { return _TMEOutput_WRTM_Alerts ?? (_TMEOutput_WRTM_Alerts = new EfRepository<TMEOutput_WRTM_Alerts>(_context)); }
      //  }


      //  public IRepository<RoadWeatherProbeInputs> RoadWeatherProbeInputs
      //  {
      //      get { return _RoadWeatherProbeInputs ?? (_RoadWeatherProbeInputs = new EfRepository<RoadWeatherProbeInputs>(_context)); }
      //  }
      //  public IRepository<EDMSSAlert> EDMSSAlerts
      //  {
      //      get { return _EDMSSAlerts ?? (_EDMSSAlerts = new EfRepository<EDMSSAlert>(_context)); }
      //  }
      //  public IRepository<District> Districts
      //  {
      //      get { return _Districts ?? (_Districts = new EfRepository<District>(_context)); }
      //  }
      //  public IRepository<Site> Sites
      //  {
      //      get { return _Sites ?? (_Sites = new EfRepository<Site>(_context)); }
      //  }
      //  public IRepository<SiteObservation> SiteObservations
      //  {
      //      get { return _SiteObservations ?? (_SiteObservations = new EfRepository<SiteObservation>(_context)); }
      //  }

    

   
      //  public void SaveChanges()
      //  {
      //      _context.SaveChanges();
      //  }
      //  //experiment for BSM call for detaching added entities from set. Commented out. Search .Entry
      //  public System.Data.Entity.Infrastructure.DbEntityEntry<T> Entry<T>(T entity) where T : class
      //  {
      //      return _context.Entry(entity);
      //  }
      //  //experiment for InfloMapViewer call to set initializer to null. Commented out. Search SetInitializer
      //  public void SetInitializerToNull()
      //  {
      //  System.Data.Entity.Database.SetInitializer<DbContext>(null);
      //  }

      //  EfRepository<Configuration_ESS> _Configuration_ESS;
      //  EfRepository<Configuration_INFLOThresholds> _Configuration_INFLOThresholds;
      //  EfRepository<Configuration_Roadway> _Configuration_Roadway;
      //  EfRepository<Configuration_RoadwayESS> _Configuration_RoadwayESS;
      //  EfRepository<Configuration_RoadwayLinks> _Configuration_RoadwayLinks;
      //  EfRepository<Configuration_RoadwayLinksDetectorStation> _Configuration_RoadwayLinksDetectorStation;
      //  EfRepository<Configuration_RoadwayLinksESS> _Configuration_RoadwayLinksESS;
      //  EfRepository<Configuration_RoadwayMileMarkers> _Configuration_RoadwayMileMarkers;
      //  EfRepository<Configuration_RoadwaySubLinks> _Configuration_RoadwaySubLinks;
      //  EfRepository<Configuration_TSSDetectionZone> _Configuration_TSSDetectionZone;
      //  EfRepository<Configuration_TSSDetectorStation> _Configuration_TSSDetectorStation;
      //  EfRepository<TME_CVData_Input> _TME_CVData_Input;
      //  EfRepository<TME_CVData_Input_Processed> _TME_CVData_Input_Processed;
      //  EfRepository<TME_CVData_SubLink> _TME_CVData_SubLink;
      //  EfRepository<TME_ESSData_Input> _TME_ESSData_Input;
      //  EfRepository<TME_ESSMobileData_Input> _TME_ESSMobileData_Input;
      //  EfRepository<TME_TSSData_Input> _TME_TSSData_Input;
      //  EfRepository<TME_TSSESS_Link> _TME_TSSESS_Link;
      //  EfRepository<TMEOutput_QWARN_QueueInfo> _TMEOutput_QWARN_QueueInfo;
      //  EfRepository<TMEOutput_QWARNMessage_CV> _TMEOutput_QWARNMessage_CV;
      //  EfRepository<TMEOutput_ShockWaveInformation> _TMEOutput_ShockWaveInformation;
      //  EfRepository<TMEOutput_SPDHARMMessage_CV> _TMEOutput_SPDHARMMessage_CV;
      //  EfRepository<TMEOutput_SPDHARMMessage_Infrastructure> _TMEOutput_SPDHARMMessage_Infrastructure;
      //  EfRepository<TMEOutput_WRTM_Alerts> _TMEOutput_WRTM_Alerts;

      //  EfRepository<RoadWeatherProbeInputs> _RoadWeatherProbeInputs;
      //  EfRepository<EDMSSAlert> _EDMSSAlerts;
      //  EfRepository<District> _Districts;
      //  EfRepository<Site> _Sites;
      //  EfRepository<SiteObservation> _SiteObservations;

      //  private readonly DbContext _context;


    }
}
