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
using InfloCommon.Models.GoogleMaps;
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
    [EnableCors(origins:"*", headers:"*", methods:"*")]
    public class QWarnController : ApiController
    {
        //private static IUnitOfWork srInfloDbContext;
        private static string strDatabaseConnectionString;
        private static string osmMapConnectionString;

        static QWarnController()
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

                        var qWarnlerts = srInfloDbContext.TMEOutput_QWARNMessage_CVs
                        .Where(d => d.DateGenerated >= dateSince
                           && d.ValidityDuration == Common.INFLO_VALIDITY_DURATION_ACTIVE);
                        //.Where(d => d.ValidityDuration == Common.INFLO_VALIDITY_DURATION_ACTIVE);
                        if (qWarnlerts != null)
                        {
                            foreach (var qWarn in qWarnlerts)
                            {

                                Location FoQloc = rsMapper.GetLocationForMileMarker(qWarn.RoadwayID, qWarn.FOQMMLocation);
                                if (FoQloc != null)
                                {
                                    Location BoQloc = rsMapper.GetLocationForMileMarker(qWarn.RoadwayID, qWarn.BOQMMLocation);

                                    GoogleMapsHelper gmHelper = new GoogleMapsHelper();

                                    var lineFeature = gmHelper.GetPolylineFeatureForLocation(BoQloc, FoQloc);
                                    if(lineFeature!=null)
                                        features.Features.Add(lineFeature);


                                    double dist = qWarn.BOQMMLocation - qWarn.FOQMMLocation;
                                    dist = Math.Abs(dist);


                                    var point = new Point(new GeographicPosition(BoQloc.Latitude, BoQloc.Longitude));
                                    var props = new Dictionary<string, object>();
                                    props.Add("RoadwayId", qWarn.RoadwayID);
                                    if (qWarn.SpeedInQueue.HasValue)
                                        props.Add("SpeedInQueue", qWarn.SpeedInQueue.Value);
                                    props.Add("FOQStart", FoQloc.Latitude.ToString() + "," + FoQloc.Longitude.ToString());
                                    props.Add("BOQStart", BoQloc.Latitude.ToString() + "," + BoQloc.Longitude.ToString());
                                    props.Add("FOQMMLocation", qWarn.FOQMMLocation);
                                    props.Add("BOQMMLocation", qWarn.BOQMMLocation);
                                    props.Add("DateGenerated", qWarn.DateGenerated);
                                    if (qWarn.ValidityDuration.HasValue)
                                        props.Add("ValidityDuration", qWarn.ValidityDuration.Value);

                                    if (qWarn.RateOfQueueGrowth.HasValue)
                                        props.Add("RateOfQueueGrowth", qWarn.RateOfQueueGrowth.Value);

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
