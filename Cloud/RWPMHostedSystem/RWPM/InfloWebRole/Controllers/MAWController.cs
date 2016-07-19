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
    public class MAWController : ApiController
    {
        private static string strDatabaseConnectionString;
        private static string strOsmMapModelDbConnectionString;

        static MAWController()
        {
            Trace.TraceInformation("[TRACE] Entering PikAlertEMDSSController::PikAlertEMDSSController() static initializer...");



            strDatabaseConnectionString =
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("InfloDatabaseConnectionString");

            strOsmMapModelDbConnectionString =
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("OsmMapModelDbConnectionString");

            if (strDatabaseConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve Inflo database connection string");
            }
            else if (strDatabaseConnectionString.Length <= 0)
            {
                Trace.TraceError("Inflo Database connection string empty");
            }
            //else  //connect to the database
            //{
            //    srInfloDbContext = new UnitOfWork(strDatabaseConnectionString);
            //}



            Trace.TraceInformation("[TRACE] Exiting PikAlertEMDSSController::PikAlertEMDSSController() static initializer...");
            return;
        }

        public FeatureCollection Get()
        {
;
            OsmMapModel mapModel = new OsmMapModel(strOsmMapModelDbConnectionString);

            var features = new FeatureCollection();
            using (IUnitOfWork srInfloDbContext = new UnitOfWork(strDatabaseConnectionString))
            {
                if (srInfloDbContext == null)
                {
                    Trace.TraceError("[TRACE] Error connecting to Inflo DB, database is null. PikAlertEMDSSController::Get...");
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Error connecting to Inflo DB, database is null."),
                        ReasonPhrase = "Error Inflo DB null."
                    };
                    throw new HttpResponseException(resp);
                }

                else
                {
                    try
                    {
                        var district = srInfloDbContext.Districts.FirstOrDefault();
                        if (district != null)
                        {

                            var sites = district.Sites.ToList();

                            DateTime dtTenMinAgo = DateTime.UtcNow.AddMinutes(-10);
                            var maws = srInfloDbContext.MAWOutputs.Where(s => s.AlertGenTime > dtTenMinAgo).ToList();

                            foreach (var site in sites)
                            {

                                bool addItem = false;

                                var point = new Point(new GeographicPosition(site.Latitude, site.Longitude));
                                var props = new Dictionary<string, object>();
                                props.Add("Description", site.Description);

                                MAWOutput mawOutput = maws.Where(o => o.SiteId == site.Id).OrderByDescending(o => o.AlertGenTime).FirstOrDefault();

                                if (mawOutput != null && mawOutput.ActionCode!=0)
                                {
                                    addItem = true;
                                    props.Add("ActionCode", mawOutput.ActionCode);
                                    props.Add("Action", MAWAlertCodeConverter.GetActionTextFromCode(mawOutput.ActionCode));
                                    props.Add("AlertGenTime", mawOutput.AlertGenTime);
                                    props.Add("AlertRequestTime", mawOutput.AlertRequestTime);
                                    props.Add("AlertTime", mawOutput.AlertTime);
                                    props.Add("PavementCode", mawOutput.PavementCode);
                                    props.Add("Pavement", MAWAlertCodeConverter.GetPavementAlertTextFromCode(mawOutput.PavementCode));
                                    props.Add("PrecipitationCode", mawOutput.PrecipitationCode);
                                    props.Add("Precipitation", MAWAlertCodeConverter.GetPrecipitationAlertTextFromCode(mawOutput.PrecipitationCode));
                                    props.Add("VisibilityCode", mawOutput.VisibilityCode);
                                    props.Add("Visibility", MAWAlertCodeConverter.GetVisibilityAlertTextFromCode(mawOutput.VisibilityCode));

                                    //Get mile marker

                                    var roadSegment = mapModel.tRoadSegments.Where(rs => rs.aux_id == mawOutput.Site.SiteIdName).FirstOrDefault();
                                    if (roadSegment != null && addItem)
                                    {

                                        RoadSegmentMapper rsMapper = new RoadSegmentMapper(strOsmMapModelDbConnectionString);

                                        Location startLocationSB = rsMapper.GetLocationForMileMarker("I35W_S", roadSegment.start_MileMarker);
                                        Location endLocationSB = rsMapper.GetLocationForMileMarker("I35W_S", roadSegment.end_MileMarker);

                                        Location startLocationNB = rsMapper.GetLocationForMileMarker("I35W_N", roadSegment.start_MileMarker);
                                        Location endLocationNB = rsMapper.GetLocationForMileMarker("I35W_N", roadSegment.end_MileMarker);


                                        GoogleMapsHelper gmHelper = new GoogleMapsHelper();

                                        if (roadSegment.north_bound_polyline == null)
                                        {
                                            string nbPolyline = gmHelper.GetEncodedPolylineForLocation(startLocationNB, endLocationNB);
                                            roadSegment.north_bound_polyline = nbPolyline;
                                        }

                                        if (roadSegment.south_bound_polyline == null)
                                        {
                                            string sbPolyline = gmHelper.GetEncodedPolylineForLocation(endLocationSB, startLocationSB);
                                            roadSegment.south_bound_polyline = sbPolyline;
                                        }

                                        mapModel.SaveChanges();

                                        var lineFeature = gmHelper.GetFeatureForEncodedPolyline(roadSegment.south_bound_polyline, props);

                                        if (lineFeature != null)
                                            features.Features.Add(lineFeature);

                                        lineFeature = gmHelper.GetFeatureForEncodedPolyline(roadSegment.north_bound_polyline, props);
                                        if (lineFeature != null)
                                            features.Features.Add(lineFeature);
                                    }

                                }


                                if (addItem)
                                {
                                    var feature = new Feature(point, props);
                                    features.Features.Add(feature);
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("[TRACE] Error in PikAlertEMDSSController::Get. " + ex.Message + "  " + ex.Source + "  " + ex.StackTrace);
                        var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(" Error in PikAlertEMDSSController::Get. " + ex.Message),
                            ReasonPhrase = "Error PikAlertEMDSSController"
                        };
                        throw new HttpResponseException(resp);
                    }

                    return features;

                }
            }
        }
    }
}
