using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Text;
using Ninject;
using InfloCommon.Models;
using InfloCommon.Repositories;
using RoadSegmentMapping;
using InfloCommon;
using System.Globalization;

namespace PikalertDataWorkerRole
{
    public class PikAlertWorker : BaseProcWorker
    {

        private string BaseURL = "http://vm-pikalert.cloudapp.net:8080/";
        private HttpClient HttpClient;
        private RoadSegmentMapper rsMapper;

        public override async void PerformWork()
        {
            Trace.TraceInformation("PikAlertWorker called");
            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();
            rsMapper = WorkerRole.Kernel.Get<RoadSegmentMapper>();

            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpClient.Timeout = TimeSpan.FromSeconds(15);

            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    Trace.TraceInformation("Working");

            try
            {
                //GET Vehicle Data from pikalert http://vm-pikalert.cloudapp.net:8080/latest_vehicles?path=/latest_vehicles/&state=minnesota

                Pikalert_Vehicles paVehicles = await GetPikalert_Vehicles();

                //Convert Pikalert Data to INFLO Vehicle Data
                if (paVehicles != null)
                {
                    foreach (var d in paVehicles.districts)
                    {
                        foreach (PA_Vehicles_Vehicle v in d.vehicles)
                        {

                            TME_CVData_Input cvInput = new TME_CVData_Input();
                            //look up milemarker
                            RoadSegment roadSegment =null;
                            try
                            {
                                roadSegment = rsMapper.GetNearestRoadSegment(Convert.ToDouble(v.lat),
                                    Convert.ToDouble(v.lon), Convert.ToDouble(v.heading_deg));
                            }
                            catch (Exception ex)
                            {
                                string inner = "";
                                if (ex.InnerException != null)
                                    inner = ex.InnerException.Message;
                                Trace.TraceError("Exception occurred when GetNearestRoadSegment. " +
                                    v.lat + ", " + v.lon + ", " + v.heading_deg + ".   " + 
                                               ex.Message + " " + ex.StackTrace + "      " + inner);
                            }
                            if (roadSegment != null)
                            {
                                cvInput.MMLocation = roadSegment.MileMarker;
                                cvInput.RoadwayId = roadSegment.RoadwayID;

                                cvInput.NomadicDeviceID = v.id;
                                cvInput.DateGenerated = FromUnixTimeSec(Convert.ToInt64(v.obs_time));
                                cvInput.Speed = Convert.ToDouble(v.speed_mph);
                                cvInput.Heading = Convert.ToInt16(Convert.ToDouble(v.heading_deg));
                                cvInput.Latitude = Convert.ToDouble(v.lat);
                                cvInput.Longitude = Convert.ToDouble(v.lon);
                                // cvInput.CVQueuedState = false;

                                uow.TME_CVData_Inputs.Add(cvInput);
                            }
                        }
                    }
                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception occurred when getting Vehicle Data from pikalert. " +
                               ex.Message + " " + ex.StackTrace);
            }
            //GET District Alert Data from pikalert http://vm-pikalert.cloudapp.net:8080/district_alerts?path=/district_alerts&state=minnesota_vdt
            try
            {
                PA_DistrictAlerts districtAlerts = await GetPikalert_DistrictAlerts();

                //TODO Filter sites by desc.  Should contain I-35W, 35w
                if (districtAlerts != null && districtAlerts.districts != null)
                {
                    foreach (var district in districtAlerts.districts)
                    {
                        if (district.district_name == "I35W")
                        {
                            // check exists in DB, if not 

                            var databaseDistrict = uow.Districts.Where(d => d.Name == district.district_name).FirstOrDefault();

                            if (databaseDistrict == null)
                            {
                                //add district if it is new
                                var dbEmdssAlert = new EDMSSAlert();
                                dbEmdssAlert.Date = DateTime.UtcNow;

                                databaseDistrict = new District();
                                databaseDistrict.Hr06AlertSummaryCode = district.hr06_alert_summary_code;
                                databaseDistrict.Hr24AlertSummaryCode = district.hr24_alert_summary_code;
                                databaseDistrict.Hr72AlertSummaryCode = district.hr72_alert_summary_code;
                                databaseDistrict.MaxLatitude = district.max_lat;
                                databaseDistrict.MaxLongitude = district.max_lon;
                                databaseDistrict.MinLatitude = district.min_lat;
                                databaseDistrict.MinLongitude = district.min_lon;
                                databaseDistrict.Name = district.district_name;
                                databaseDistrict.ObservAlertSummaryCode = district.obs_alert_summary_code;

                                dbEmdssAlert.Districts.Add(databaseDistrict);
                                uow.EDMSSAlerts.Add(dbEmdssAlert);
                            }

                            WeatherEventStatistics stats = new WeatherEventStatistics();
                            foreach (var site in district.sites)
                            {
                                //Check for the various ways the roadway is returned in teh dataset
                                if (site.desc.Contains("35w") || site.desc.Contains("I-35W"))
                                {
                                    var databaseSite = databaseDistrict.Sites.Where(s => s.Description == site.desc).FirstOrDefault();
                                    if (databaseSite == null)
                                    {//this site is new
                                        databaseSite = new Site();
                                        databaseSite.Description = site.desc;
                                        databaseSite.Hr06AlertSummaryCode = site.hr06_alert_code;
                                        databaseSite.Hr24AlertSummaryCode = site.hr24_alert_code;
                                        databaseSite.Hr72AlertSummaryCode = site.hr72_alert_code;
                                        databaseSite.IsRoadCondSite = site.is_road_cond_site;
                                        databaseSite.IsWeatherObservSite = site.is_wx_obs_site;
                                        databaseSite.Latitude = site.lat;
                                        databaseSite.Longitude = site.lon;
                                        databaseSite.ObservAlertCode = site.obs_alert_code;
                                        databaseSite.SiteIdName = site.site_id;
                                        databaseSite.SiteNum = site.site_num;
                                        
                                        //hardcoded to look for both directions of above-harcoded i35
                                        databaseSite.MileMarker = rsMapper.GetNearestMileMarkerEitherDirection
                                            (databaseSite.Latitude, databaseSite.Longitude, "I35W_N", "I35W_S");


                                        if (site.time_series.Count > 0)
                                        {//add observation for site
                                            SiteObservation siteObs = new SiteObservation();
                                            siteObs.AlertCode = site.time_series[0].alert_code;
                                            siteObs.Chemical = site.time_series[0].chemical;
                                            siteObs.DateTime = DateTime.ParseExact(site.time_series[0].time, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                                            siteObs.Pavement = site.time_series[0].pavement;
                                            siteObs.Plow = site.time_series[0].plow;
                                            siteObs.Precipitation = site.time_series[0].precip;
                                            siteObs.RoadTemp = site.time_series[0].road_temp;
                                            siteObs.TreatmentAlertCode = site.time_series[0].treatment_alert_code;
                                            siteObs.Visibility = site.time_series[0].visibility;

                                            databaseSite.SiteObservations.Add(siteObs);
                                            stats.AddToStatistics(siteObs);
                                        }
                                        databaseDistrict.Sites.Add(databaseSite);
                                    }
                                    else
                                    {//site already existed in db.
                                        databaseSite.Hr06AlertSummaryCode = site.hr06_alert_code;
                                        databaseSite.Hr24AlertSummaryCode = site.hr24_alert_code;
                                        databaseSite.Hr72AlertSummaryCode = site.hr72_alert_code;

                                        if (site.time_series.Count > 0)
                                        {
                                            DateTime dtTime = DateTime.ParseExact(site.time_series[0].time, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                                            var dbTimeSeries = databaseSite.SiteObservations.Where(o => o.DateTime == dtTime).FirstOrDefault();
                                            //See if we already recorded this observation for this site.
                                            if (dbTimeSeries == null)
                                            {
                                                SiteObservation siteObs = new SiteObservation();
                                                siteObs.AlertCode = site.time_series[0].alert_code;
                                                siteObs.Chemical = site.time_series[0].chemical;
                                                siteObs.DateTime = DateTime.ParseExact(site.time_series[0].time, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                                                siteObs.Pavement = site.time_series[0].pavement;
                                                siteObs.Plow = site.time_series[0].plow;
                                                siteObs.Precipitation = site.time_series[0].precip;
                                                siteObs.RoadTemp = site.time_series[0].road_temp;
                                                siteObs.TreatmentAlertCode = site.time_series[0].treatment_alert_code;
                                                siteObs.Visibility = site.time_series[0].visibility;

                                                if (String.IsNullOrEmpty(siteObs.Pavement))
                                                {
                                                    //Sometimes we get empty data from PikAlert. Don't add this to the database.
                                                    Trace.TraceError("siteObs Pavement is null or empty. Skipping." +
                                                        databaseSite.Id + " " +
                                                         siteObs.DateTime);
                                                }
                                                else
                                                {
                                                    databaseSite.SiteObservations.Add(siteObs);

                                                    stats.AddToStatistics(siteObs);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            stats.ProcessWeatherEvents(uow);
                            Trace.TraceError(string.Format("Tempmsg. Pavement: {0},{1},{2},{3} Precip: {4},{5},{6},{7}", 
                                stats.TrackPavementCondCnts[0],
                                stats.TrackPavementCondCnts[1],
                                stats.TrackPavementCondCnts[2],
                                stats.TrackPavementCondCnts[3],
                                 stats.TrackPrecipCondCnts[0],
                                stats.TrackPrecipCondCnts[1],
                                stats.TrackPrecipCondCnts[2],
                                stats.TrackPrecipCondCnts[3]));
                        }
                        uow.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception occurred when getting District Data from pikalert. " +
                               ex.Message + " " + ex.StackTrace);
            }

            //    await Task.Delay(5 * 1000);
            //}
        }


        private DateTime FromUnixTimeSec(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }


        private async Task<Pikalert_Vehicles> GetPikalert_Vehicles()
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("path", "/latest_vehicles/");
                parameters.Add("state", "minnesota");

                return await GetFromWebService<Pikalert_Vehicles>("/latest_vehicles", parameters);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<PA_DistrictAlerts> GetPikalert_DistrictAlerts()
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("path", "/district_alerts");
                parameters.Add("state", "minnesota_vdt");

                return await GetFromWebService<PA_DistrictAlerts>("/district_alerts", parameters);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected async Task<T> GetFromWebService<T>(String relativeUrl, Dictionary<string, object> parameters)
        {
            try
            {
                String url = BaseURL + relativeUrl;
                url = AddUrlParams(url, parameters);
                HttpResponseMessage response = await HttpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var returnData = JsonConvert.DeserializeObject<T>(responseString);

                    return returnData;
                }
                else
                {
                    String message = "Get error status code: " + response.StatusCode + " ; body: " + response.Content.ToString();
                    throw new Exception(message);
                }
            }
            catch (HttpRequestException)
            {
                throw new TimeoutException();
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }
        }

        private static string AddUrlParams(string baseUrl, Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                var stringBuilder = new StringBuilder(baseUrl);
                var hasFirstParam = baseUrl.Contains("?");

                foreach (var parameter in parameters)
                {
                    var format = hasFirstParam ? "&{0}={1}" : "?{0}={1}";
                    stringBuilder.AppendFormat(format, Uri.EscapeDataString(parameter.Key),
                        Uri.EscapeDataString(parameter.Value.ToString()));
                    hasFirstParam = true;
                }

                return stringBuilder.ToString();
            }
            else
                return baseUrl;
        }








    }


}
