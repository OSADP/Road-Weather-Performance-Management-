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
    public class PikAlertEMDSSController : ApiController
    {

        private static string strDatabaseConnectionString;
        private static string strOsmMapModelDbConnectionString;

        static PikAlertEMDSSController()
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

        public FeatureCollection Get(bool returnPlowOtherThanNone = false, bool returnChemicalOtherThanNone = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            OsmMapModel mapModel = new OsmMapModel(strOsmMapModelDbConnectionString);
            double time1, time2, time3, time4, time5, time6, time7, time8, time9;
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
                        time1 = sw.Elapsed.TotalSeconds;
                        var district = srInfloDbContext.Districts.FirstOrDefault();
                        if (district != null)
                        {
                            time2 = sw.Elapsed.TotalSeconds;
                            var sites = district.Sites.ToList();
                            time3 = sw.Elapsed.TotalSeconds;
                            DateTime dtThirtyMinutesAgo = DateTime.UtcNow.AddMinutes(-120);
                            var observations = srInfloDbContext.SiteObservations.Where(s => s.DateTime > dtThirtyMinutesAgo).ToList();
                            time4 = sw.Elapsed.TotalSeconds;
                            foreach (var site in sites)
                            {

                                bool addItem = true;

                                if (returnChemicalOtherThanNone == true || returnPlowOtherThanNone == true)
                                {
                                    addItem = false;
                                }

                                var point = new Point(new GeographicPosition(site.Latitude, site.Longitude));
                                var props = new Dictionary<string, object>();
                                props.Add("Description", site.Description);

                                SiteObservation observation = observations.Where(o => o.SiteId == site.Id).OrderByDescending(o => o.DateTime).FirstOrDefault();


                                if (observation != null)
                                {
                                    props.Add("Observation Time", observation.DateTime);
                                    props.Add("AlertCode", observation.AlertCode);
                                    props.Add("Chemical", observation.Chemical);
                                    props.Add("Pavement", observation.Pavement);
                                    props.Add("Plow", observation.Plow);
                                    props.Add("Precipitation", observation.Precipitation);
                                    props.Add("RoadTemp", observation.RoadTemp);
                                    props.Add("TreatmentAlertCode", observation.TreatmentAlertCode);
                                    props.Add("Visibility", observation.Visibility);


                                    

                                    if (returnPlowOtherThanNone == true && observation.Plow != "none")
                                    {
                                        //Don't return this site's latest feedback, since their plow status is none.
                                        addItem = true;
                                    }

                                    if (returnChemicalOtherThanNone == true && observation.Chemical != "none")
                                    {
                                        //Don't return this site's latest feedback, since their chemical status is none.
                                        addItem = true;
                                    }

                                    //Get mile marker

                                    time5 = sw.Elapsed.TotalSeconds;
                                    var roadSegment = mapModel.tRoadSegments.Where(rs => rs.aux_id == observation.Site.SiteIdName).FirstOrDefault();
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
                                    time6 = sw.Elapsed.TotalSeconds;


                                   

                                    time7 = sw.Elapsed.TotalSeconds;
                                }


                                if (addItem)
                                {
                                    var feature = new Feature(point, props);
                                    features.Features.Add(feature);
                                }

                                time8 = sw.Elapsed.TotalSeconds;
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

                    time9 = sw.Elapsed.TotalSeconds;
                    return features;

                }
            }
        }

        public FeatureCollection Get(int weatherEventId, int siteId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            OsmMapModel mapModel = new OsmMapModel(strOsmMapModelDbConnectionString);
            double time1, time2, time3, time4, time5, time6, time7, time8, time9;
            var features = new FeatureCollection();
            using (IUnitOfWork uow = new UnitOfWork(strDatabaseConnectionString))
            {
                if (uow == null)
                {
                    Trace.TraceError("[TRACE] Error connecting to Inflo DB, database is null. PikAlertEMDSSController::Get...");
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Error connecting to Inflo DB, database is null."),
                        ReasonPhrase = "Error Inflo DB null."
                    };
                    throw new HttpResponseException(resp);
                }


                try
                {
                    time1 = sw.Elapsed.TotalSeconds;
                    var district = uow.Districts.FirstOrDefault();
                    if (district != null)
                    {
                        WeatherEvent weatherEv = uow.WeatherEvents.Where(w => w.Id == weatherEventId).FirstOrDefault();
                        Site site = district.Sites.Where(o => o.Id == siteId).FirstOrDefault();
                        if (weatherEv != null && site != null)
                        {

                            time2 = sw.Elapsed.TotalSeconds;
                            time3 = sw.Elapsed.TotalSeconds;
                            DateTime dtSince = weatherEv.StartTime;
                            DateTime dtTo = DateTime.UtcNow;
                            if (weatherEv.EndTime != null) dtTo = (DateTime)weatherEv.EndTime;
                            time4 = sw.Elapsed.TotalSeconds;

                           
                            var point = new Point(new GeographicPosition(site.Latitude, site.Longitude));
                            var props = new Dictionary<string, object>();
                            props.Add("Description", site.Description);

                            //var siteObservations = observations.Where(o => o.SiteId == site.Id);
                              var observations = uow.SiteObservations.Where(s => s.DateTime >= dtSince && s.DateTime <= dtTo);
                        var siteObservations = observations.Where(o => o.SiteId == siteId);

                            props.Add("Road Temperature", siteObservations.Average(s => s.RoadTemp));




                            if (siteObservations != null)
                            {
                                WeatherEventStatistics stats = new WeatherEventStatistics();
                                foreach (var siteobs in siteObservations)
                                {
                                    stats.AddToStatistics(siteobs);
                                }

                                //props.Add("Percent Obs Reported Clear", stats.CalculatePercentClear());
                                double clear, wet, snow, ice;
                                stats.CalculatePavementPercentages(out ice, out snow, out wet, out clear);
                                props.Add("Percent Pavement Obs Reported Clear", Math.Round(clear));
                                props.Add("Percent Pavement Obs Reported Wet", Math.Round(wet));
                                props.Add("Percent Pavement Obs Reported Snow", Math.Round(snow));
                                props.Add("Percent Pavement Obs Reported Ice", Math.Round(ice));
                                stats.CalculatePrecipPercentages(out ice, out snow, out wet, out clear);
                                props.Add("Percent Precipitation Obs Reported Clear", Math.Round(clear));
                                props.Add("Percent Precipitation Obs Reported Wet", Math.Round(wet));
                                props.Add("Percent Precipitation Obs Reported Snow", Math.Round(snow));
                                props.Add("Percent Precipitation Obs Reported Ice", Math.Round(ice));
                               
                                time5 = sw.Elapsed.TotalSeconds;
                                //Grab the location for this site
                                string siteIdName = siteObservations.First().Site.SiteIdName;
                                var roadSegment = mapModel.tRoadSegments.Where(rs => rs.aux_id == siteIdName).FirstOrDefault();
                                if (roadSegment != null)
                                {

                                    RoadSegmentMapper rsMapper = new RoadSegmentMapper(strOsmMapModelDbConnectionString);
                                    string southRoadwayId = "I35W_S";
                                    string northRoadwayId = "I35W_N";
                                    //Get mile markers for the site
                                    Location startLocationSB = rsMapper.GetLocationForMileMarker(southRoadwayId, roadSegment.start_MileMarker);
                                    Location endLocationSB = rsMapper.GetLocationForMileMarker(southRoadwayId, roadSegment.end_MileMarker);

                                    Location startLocationNB = rsMapper.GetLocationForMileMarker(northRoadwayId, roadSegment.start_MileMarker);
                                    Location endLocationNB = rsMapper.GetLocationForMileMarker(northRoadwayId, roadSegment.end_MileMarker);

                                    //Query detector stations for this milemarkers
                                    double southAvgSpeed = GetAvgSpeedForRoadSegment(uow, roadSegment, southRoadwayId);
                                    double northAvgSpeed = GetAvgSpeedForRoadSegment(uow, roadSegment, northRoadwayId);
                                    props.Add("South Avg Speed", southAvgSpeed);
                                    props.Add("North Avg Speed", northAvgSpeed);

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
                                time6 = sw.Elapsed.TotalSeconds;




                                time7 = sw.Elapsed.TotalSeconds;
                            }


                         

                            time8 = sw.Elapsed.TotalSeconds;
                            // }
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

                time9 = sw.Elapsed.TotalSeconds;
                return features;


            }
        }

        /// <summary>
        /// Get site locations in pik alert for the weather event
        /// </summary>
        /// <param name="weatherEventId">(Future use  - we should have roadway id for the weather event and look up the right site locations.)</param>
        /// <returns></returns>
        public FeatureCollection Get(int weatherEventId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
         
            double time1, time9;
            var features = new FeatureCollection();
            using (IUnitOfWork uow = new UnitOfWork(strDatabaseConnectionString))
            {
                if (uow == null)
                {
                    Trace.TraceError("[TRACE] Error connecting to Inflo DB, database is null. PikAlertEMDSSController::Get...");
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Error connecting to Inflo DB, database is null."),
                        ReasonPhrase = "Error Inflo DB null."
                    };
                    throw new HttpResponseException(resp);
                }

                try
                {
                    time1 = sw.Elapsed.TotalSeconds;
                    var district = uow.Districts.FirstOrDefault();
                    if (district != null)
                    {
                        var sites = district.Sites.ToList();
                        foreach (var site in sites)
                        {
                            var point = new Point(new GeographicPosition(site.Latitude, site.Longitude));
                            var props = new Dictionary<string, object>();
                            props.Add("siteId", site.Id);

                            Feature feature = new Feature(point, props, site.Id.ToString());                        
                            features.Features.Add(feature);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("[TRACE] Error in PikAlertEMDSSController::GetSiteLocations. " + ex.Message + "  " + ex.Source + "  " + ex.StackTrace);
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(" Error in PikAlertEMDSSController::GetSiteLocations. " + ex.Message),
                        ReasonPhrase = "Error PikAlertEMDSSController"
                    };
                    throw new HttpResponseException(resp);
                }

                time9 = sw.Elapsed.TotalSeconds;
                return features;
            }
        }

        private static double GetAvgSpeedForRoadSegment(IUnitOfWork uow, tRoadSegment roadSegment, string southRoadwayId)
        {
            var southStations = uow.Configuration_TSSDetectorStations.Where(s => s.MMLocation > roadSegment.start_MileMarker
                && s.MMLocation <= roadSegment.end_MileMarker
                && s.RoadwayId == southRoadwayId).Select(s => s.DSId);

            var southZones = uow.Configuration_TSSDetectionZones.Where(z => southStations.Contains(z.DSId)).Select(s => s.DZId);
            if (uow.TME_TSSData_Inputs.Where(t => southZones.Contains(t.DZId)).FirstOrDefault() == null)
            {
                //These stations don't have any data.
                return -1;
            }
            return uow.TME_TSSData_Inputs.Where(t => southZones.Contains(t.DZId)).Average(t => t.AvgSpeed);
        }

    }
}
