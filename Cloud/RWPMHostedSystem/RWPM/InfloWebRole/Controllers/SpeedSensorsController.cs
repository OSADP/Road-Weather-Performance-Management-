using System;
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
using RoadSegmentMapping;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Web.Http.Cors;
using System.Collections.Generic;

namespace InfloWebRole.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SpeedSensorsController : ApiController
    {
        private static string strDatabaseConnectionString;

        static SpeedSensorsController()
        {
            strDatabaseConnectionString =
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("InfloDatabaseConnectionString");

            if (strDatabaseConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve Inflo database connection string");
            }
            else if (strDatabaseConnectionString.Length <= 0)
            {
                Trace.TraceError("Inflo Database connection string empty");
                strDatabaseConnectionString = null;
            }
        }

        public FeatureCollection Get()
        {
            var features = new FeatureCollection();
            using (IUnitOfWork srInfloDbContext = new UnitOfWork(strDatabaseConnectionString))
            {
                if (srInfloDbContext == null)
                {
                    Trace.TraceError("[TRACE] Error connecting to Inflo DB, database is null. MotoristAlertController::Get...");
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Error connecting to Inflo DB, database is null."),
                        ReasonPhrase = "Error Inflo DB null."
                    };
                    throw new HttpResponseException(resp);
                }
                else
                {
                    var speedSensors = srInfloDbContext.Configuration_TSSDetectionZones.Where(dz => dz.RoadwayId.Contains("I35W")).ToList();
                    var detectorStations = srInfloDbContext.Configuration_TSSDetectorStations.ToList();
                    var latestSpeedObservations = srInfloDbContext.TME_TSSData_Inputs.OrderByDescending(d => d.DateReceived).Take(300).ToList();
                    foreach(var detectionZone in speedSensors)
                    {
                        var detectorStation = detectorStations.Where(ds => ds.DSId == detectionZone.DSId).FirstOrDefault();
                        var latestObservation = latestSpeedObservations.Where(o => o.DZId == detectionZone.DZId).OrderByDescending(d => d.BeginTime).FirstOrDefault();
                            //srInfloDbContext.TME_TSSData_Inputs.Where(o => o.DZId == detectionZone.DZId).OrderByDescending(d => d.DateReceived).FirstOrDefault();
                        if (detectorStation!=null && latestObservation != null)
                        {

                            var point = new Point(new GeographicPosition(detectorStation.Latitude.Value, detectorStation.Longitude.Value));
                            var props = new Dictionary<string, object>();
                            props.Add("DZId", latestObservation.DZId);
                            props.Add("BeginTime", latestObservation.BeginTime);
                            props.Add("Volume", latestObservation.Volume);
                            props.Add("AvgSpeed", latestObservation.AvgSpeed);
                            props.Add("Congested", latestObservation.Congested);
                            props.Add("Occupancy", latestObservation.Occupancy);

                            var feature = new Feature(point, props);
                            features.Features.Add(feature);
                        }

                    }


                }
            }

            return features;
        }
    }

}