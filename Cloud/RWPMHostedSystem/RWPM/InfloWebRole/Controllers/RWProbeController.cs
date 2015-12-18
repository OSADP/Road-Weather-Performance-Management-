using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using InfloCommon.Models;
using InfloCommon;
using InfloCommon.Repositories;
using System.Xml;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Web.Http.Cors;

namespace InfloWebRole.Controllers
{
    /// <summary>
    /// Receives RoadWeatherProbe data input
    /// </summary>
    /// 
    [EnableCors(origins:"*", headers:"*", methods:"*")]
    public class RWProbeController : ApiController
    {
        private static Microsoft.WindowsAzure.Storage.CloudStorageAccount srStorageAccount;
        private static Microsoft.WindowsAzure.Storage.Queue.CloudQueueClient srCloudQueueClient;
        private static Microsoft.WindowsAzure.Storage.Queue.CloudQueue srProbeQueue;
        private static string queueName = "inbound-probes";


        private static string strDatabaseConnectionString;

        private static Dictionary<String, DateTime> LastTimeRecordedDictionary;
         //  private static IUnitOfWork srInfloDbContext;

        static RWProbeController()
        {
            Trace.TraceInformation("[TRACE] Entering RWProbeController::RWProbeController() static initializer...");

            LastTimeRecordedDictionary = new Dictionary<string, DateTime>();

            string strStorageAccountConnectionString =
        Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("StorageAccountConnectionString");

            if (strStorageAccountConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve storage account connection string");
            }
            else if (strStorageAccountConnectionString.Length <= 0)
            {
                Trace.TraceError("Storage account connection string empty");
            }
            else  //connect to the cloud storage account
            {
                try
                {
                    srStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(strStorageAccountConnectionString);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Exception occurred when parsing storage account connection string\n{0}\n{1}",
                        strStorageAccountConnectionString, e.Message);
                }

                if (srStorageAccount != null)
                {
                    srCloudQueueClient = srStorageAccount.CreateCloudQueueClient();
                    srProbeQueue = srCloudQueueClient.GetQueueReference(queueName);

                    try
                    {
                        if (srProbeQueue.CreateIfNotExists())
                        {
                            Trace.TraceInformation("Created Azure Probe queue '{0}'", queueName);
                        }
                        else
                        {
                            Trace.TraceInformation("Got reference to existing Probe queue '{0}'", queueName);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Exception occurred when creating queue for inbound Probes\n{0}",
                            e.Message);

                        srProbeQueue = null;
                    }
                }
            }

            strDatabaseConnectionString =
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("InfloDatabaseConnectionString");

            if (strDatabaseConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve database connection string");
            }
            else if (strDatabaseConnectionString.Length <= 0)
            {
                strDatabaseConnectionString = null;
                Trace.TraceError("Database connection string empty");
            }
            //else  //connect to the database
            //{
            //    srInfloDbContext = new UnitOfWork(strDatabaseConnectionString);
            //}


            Trace.TraceInformation("[TRACE] Exiting RWProbeController::RWProbeController() static initializer...");
            return;
        }

        public List<RoadWeatherProbeInputs> Get(DateTime utcDataSince)
        {
               Trace.TraceInformation("[TRACE] Entering RWProbeController::Get...");

               if (strDatabaseConnectionString == null)
               {
                   Trace.TraceError("[TRACE] Error connecting to Inflo DB, database is null. RWProbeController::Get...");
                   return null;
               }
               using (IUnitOfWork srInfloDbContext = new UnitOfWork(strDatabaseConnectionString))
               {
                   return srInfloDbContext.RoadWeatherProbeInputs.Where(d => d.DateGenerated >= utcDataSince).OrderBy(d=>d.DateGenerated).ToList();
               }         
        }

        public FeatureCollection Get()
        {
            DateTime dtUCT = DateTime.UtcNow.AddSeconds(-120);

            var features = new FeatureCollection();

            if (strDatabaseConnectionString == null)
            {
                Trace.TraceError("[TRACE] Error connecting to Inflo DB, database is null. RWProbeController::Get...");
                return null;
            }
            using (IUnitOfWork srInfloDbContext = new UnitOfWork(strDatabaseConnectionString))
            {
                var probeInputs = srInfloDbContext.RoadWeatherProbeInputs.Where(d => d.DateGenerated >= dtUCT).OrderByDescending(d => d.DateGenerated)
                    .GroupBy(d=>d.NomadicDeviceId).ToList();

                foreach(var nomaticDeviceProbes in probeInputs)
                {
                    var probeInput = nomaticDeviceProbes.FirstOrDefault();

                    var point = new Point(new GeographicPosition(probeInput.GpsLatitude, probeInput.GpsLongitude,probeInput.GpsElevation));
                    var props = new Dictionary<string, object>();
                    props.Add("GpsHeading", probeInput.GpsHeading);
                    props.Add("GpsSpeed", probeInput.GpsSpeed);
                    props.Add("HeadlightStatus", probeInput.HeadlightStatus);
                    props.Add("AirTemperature", probeInput.AirTemperature);
                    props.Add("AtmosphericPressure", probeInput.AtmosphericPressure);
                    props.Add("LeftFrontWheelSpeed", probeInput.LeftFrontWheelSpeed);
                    props.Add("LeftRearWheelSpeed", probeInput.LeftRearWheelSpeed);
                    props.Add("NomadicDeviceId", probeInput.NomadicDeviceId);
                    props.Add("RightFrontWheelSpeed", probeInput.RightFrontWheelSpeed);
                    props.Add("RightRearWheelSpeed", probeInput.RightRearWheelSpeed);
                    props.Add("Speed", probeInput.Speed);
                    props.Add("SteeringWheelAngle", probeInput.SteeringWheelAngle);
                    props.Add("WiperStatus", probeInput.WiperStatus);
                    props.Add("DateGenerated", probeInput.DateGenerated);
                    
                    var feature = new Feature(point, props);
                    features.Features.Add(feature);
                }
            }  


            return features;
        }

        public List<RoadWeatherProbeInputs> Get(int roadWeatherProbeInputsId)
        {
            Trace.TraceInformation("[TRACE] Entering RWProbeController::Get by ID...");

            if (strDatabaseConnectionString == null)
            {
                Trace.TraceError("[TRACE] Error connecting to Inflo DB, database is null. RWProbeController::Get...");
                return null;
            }
            using (IUnitOfWork srInfloDbContext = new UnitOfWork(strDatabaseConnectionString))
            {
                return srInfloDbContext.RoadWeatherProbeInputs.Where(d => d.Id >= roadWeatherProbeInputsId).OrderBy(d => d.Id).ToList();
            }
        }

        /// <summary>
        /// Posts data to queue for BSM worker.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(List<RWProbeModel> p)
        {
            try
            {
                Trace.TraceInformation("[TRACE] Entering RWProbeController::Post...");
                if (srProbeQueue == null)
                {
                    Trace.TraceError("Unable to add message to queue-- Probe queue not created");
                    Trace.TraceInformation("[TRACE] Exiting RWProbeController::Post()...");
                    return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Inbound Probe queue does not exist");
                }

                if (!ModelState.IsValid)
                {
                    //problem auto binding.
                    Exception ex = ModelState.First().Value.Errors[0].Exception;
                    Trace.TraceError("[TRACE] Error binding RWProbeController::Post. " + ex.Message + "  " + ex.Source + "  " + ex.StackTrace);
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error in RWProbeController::Post.  " + ex.Message);
                }

                foreach (RWProbeModel m in p)
                {
                    //Filter probe data inputs.  Only put ones on the queue that are >= 2 seconds since last recorded
                    bool isRecordRWProbeModel = false;
                    if(LastTimeRecordedDictionary.Keys.Contains(m.NomadicDeviceId) == true)
                    {
                        DateTime dtLastRecorded = LastTimeRecordedDictionary[m.NomadicDeviceId];

                        TimeSpan tsRecordedTimeDiff = m.DateGenerated - dtLastRecorded;
                        if(tsRecordedTimeDiff.TotalSeconds >= 2)
                        {
                            isRecordRWProbeModel = true;
                        }
                    }
                    else
                    {
                        isRecordRWProbeModel = true;
                    }

                    if (isRecordRWProbeModel) 
                    {
                        if (LastTimeRecordedDictionary.Keys.Contains(m.NomadicDeviceId) == true)
                        {
                            LastTimeRecordedDictionary[m.NomadicDeviceId] = m.DateGenerated;
                        }
                        else
                        {
                            LastTimeRecordedDictionary.Add(m.NomadicDeviceId, m.DateGenerated);
                        }

                        var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(m);
                        string strQueueMessage = Newtonsoft.Json.JsonConvert.SerializeObject(json);
                        Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage rMessage =
                            new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(strQueueMessage);

                        const int hours = 1;
                        const int mins = 0;
                        const int secs = 0;
                        TimeSpan rTimeToLive = new TimeSpan(hours, mins, secs);

                        Trace.TraceInformation("Adding Probe message to inbound probes queue...");
                        srProbeQueue.AddMessage(rMessage, rTimeToLive);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.NoContent);

            }
            catch (Exception ex)
            {
                Trace.TraceError("[TRACE] Error in RWProbeController::Post. " + ex.Message + "  " + ex.Source + "  " + ex.StackTrace);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error in RWProbeController::Post.  " + ex.Message);
            }
        }


    }
}
