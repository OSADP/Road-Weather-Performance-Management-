
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
using RoadSegmentMapping;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Web.Http.Cors;
namespace InfloWebRole.Controllers
{
    

        [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SpeedHarmController : ApiController
    {
            
    //private static IUnitOfWork srInfloDbContext;
    private static string strDatabaseConnectionString;

        private static string osmMapConnectionString;

        static SpeedHarmController()
        {
            Trace.TraceInformation("[TRACE] Entering MotoristAlertController::MotoristAlertController() static initializer...");

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
            //else  //connect to the database
            //{
            //    srInfloDbContext = new UnitOfWork(strDatabaseConnectionString);
            //}

            osmMapConnectionString =
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("OsmMapModelDbConnectionString");
            if (osmMapConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve OsmMapModel database connection string");
            }
            else if (osmMapConnectionString.Length <= 0)
            {
                Trace.TraceError("OsmMapModel Database connection string empty");
                osmMapConnectionString = null;
            }

            Trace.TraceInformation("[TRACE] Exiting MotoristAlertController::MotoristAlertController() static initializer...");
            return;
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
                else if (osmMapConnectionString == null)
                {
                    Trace.TraceError("[TRACE] Error connecting to OsmMapModel DB, database string is null. MotoristAlertController::Get...");
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Error connecting to OsmMapModel DB, database connection string is null."),
                        ReasonPhrase = "Error OsmMapModel DB null."
                    };
                    throw new HttpResponseException(resp);
                }
                else
                {
                    try
                    {

                        //Translate Lat/Long/Heading into Roadway ID and MM.
                        RoadSegmentMapper rsMapper = new RoadSegmentMapper(osmMapConnectionString);

                        //Query qWarn table for alerts

                        //Grab the latest alerts that are 'still valid'
                        DateTime dateSince = DateTime.UtcNow.AddMinutes(-1 * Common.INFLO_VALIDITY_DURATION_ACTIVE);

                        //To accurately get 'valid' warnings we'd need to compare the current date time to 
                        // DateGenerated + ValidityDuration
                        //for every row in the database.  To simplify, we know the validity duration is always 60, so we can do that on this
                        //side and then compare the DateGenerated only to the hour-old value.
                        //However, the Admin Traffic Controllers can "turn off" warnings, by setting the ValidityDuration to 0 - so for 
                        //our scheme we need to make sure the ValidityDuration is its untouched original state.
                        var sdHrmAlerts = srInfloDbContext.TMEOutput_SPDHARMMessage_CVs
                       .Where(d => d.DateGenerated >= dateSince
                           && d.ValidityDuration == Common.INFLO_VALIDITY_DURATION_ACTIVE);
 //.Where(d => d.ValidityDuration == Common.INFLO_VALIDITY_DURATION_ACTIVE);

       


                        if (sdHrmAlerts != null)
                        {
                            foreach (var spdH in sdHrmAlerts)
                            {
                                Location beginLoc = rsMapper.GetLocationForMileMarker(spdH.RoadwayId, spdH.BeginMM);
                                if (beginLoc != null)
                                {
                                   
                                    var point = new Point(new GeographicPosition(beginLoc.Latitude, beginLoc.Longitude));
                                    var props = new Dictionary<string, object>();
                                    props.Add("RoadwayId", spdH.RoadwayId);
                                    props.Add("RecommendedSpeed", spdH.RecommendedSpeed);
                                    props.Add("BeginMM", spdH.BeginMM);
                                    props.Add("Justification", spdH.Justification);
                                    props.Add("BeginLoc", beginLoc.Latitude.ToString() + "," + beginLoc.Longitude.ToString());
                                    if (spdH.EndMM.HasValue)
                                    {
                                        Location endloc = rsMapper.GetLocationForMileMarker(spdH.RoadwayId, spdH.EndMM.Value);

                                        props.Add("EndLoc", endloc.Latitude.ToString() + "," + endloc.Longitude.ToString());
                                        props.Add("EndMM", spdH.EndMM);
                                        double dist = spdH.EndMM.Value - spdH.BeginMM;
                                        dist = Math.Abs(dist);
                                        props.Add("Distance", dist);
                                    }

                                    var feature = new Feature(point, props);
                                    features.Features.Add(feature);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("[TRACE] Error in MotoristAlertController::Get. " + ex.Message + "  " + ex.Source + "  " + ex.StackTrace);
                        var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(" Error in MotoristAlertController::Get. " + ex.Message),
                            ReasonPhrase = "Error MotoristAlertController"
                        };
                        throw new HttpResponseException(resp);
                    }

                    return features;
                }
            }
        }
    }
}
