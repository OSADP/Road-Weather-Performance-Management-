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
using System.IO;

namespace PikalertDataWorkerRole
{
    public class PikAlertWorker : BaseProcWorker
    {

        private string BaseURL = "http://vm-pikalert.cloudapp.net:8080/";
        private HttpClient HttpClient;
        private RoadSegmentMapper rsMapper;
        private int currentFileInteger = 1;

        public override async void PerformWork()
        {
            Trace.TraceInformation("PikAlertWorker called");
            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();
            rsMapper = WorkerRole.Kernel.Get<RoadSegmentMapper>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpClient.Timeout = TimeSpan.FromSeconds(15);

            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    Trace.TraceInformation("Working");

            try
            {
                //GET Vehicle Data from pikalert http://vm-pikalert.cloudapp.net:8080/latest_vehicles?path=/latest_vehicles/&state=minnesota

                Pikalert_Vehicles paVehicles = null;// await GetPikalert_Vehicles();

                //Convert Pikalert Data to INFLO Vehicle Data
                if (paVehicles != null)
                {
                    foreach (var d in paVehicles.districts)
                    {
                        foreach (PA_Vehicles_Vehicle v in d.vehicles)
                        {

                            TME_CVData_Input cvInput = new TME_CVData_Input();
                            RoadWeatherProbeInputs rwpInput = new RoadWeatherProbeInputs();


                            rwpInput.AirTemperature = Convert.ToDouble(v.temp_f);
                            rwpInput.DateCreated = FromUnixTimeSec(Convert.ToInt64(v.obs_time));
                            rwpInput.DateGenerated = FromUnixTimeSec(Convert.ToInt64(v.obs_time));
                            rwpInput.GpsElevation = 0;
                            rwpInput.GpsHeading = Convert.ToInt16(Convert.ToDouble(v.heading_deg));
                            rwpInput.GpsLatitude = Convert.ToDouble(v.lat);
                            rwpInput.GpsLongitude = Convert.ToDouble(v.lon);
                            rwpInput.GpsSpeed = Convert.ToDouble(v.speed_mph);

                            bool isWipersOn = false;
                            if (Convert.ToInt32(v.wiper_status) > 0)
                                isWipersOn = true;

                            rwpInput.WiperStatus = isWipersOn;
                            rwpInput.NomadicDeviceId = v.id;


                            uow.RoadWeatherProbeInputs.Add(rwpInput);

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

            long ms = sw.ElapsedMilliseconds;
            Console.WriteLine("PikAlert Worker Role Elapsed MS before DA : " + ms.ToString());
            try
            {
                PA_DistrictAlerts districtAlerts = await GetPikalert_DistrictAlerts(); //await GetPikalert_DistrictAlertsFromFiles(); 

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
                                            SiteObservation siteObs = RecordSiteObservation(stats, site, databaseSite);
                                            if(siteObs!= null)
                                                databaseSite.MAWOutputs.Add(await AddMAWForSite(site, siteObs.Precipitation, siteObs.Pavement, siteObs.AlertCode));
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
                                                SiteObservation siteObs = RecordSiteObservation(stats, site, databaseSite);
                                                if(siteObs!=null)
                                                    databaseSite.MAWOutputs.Add(await AddMAWForSite(site, siteObs.Precipitation, siteObs.Pavement, siteObs.AlertCode));
                                            }
                                        }

                                        
                                        databaseDistrict.Sites.Add(databaseSite);
                                    }
                                }
                            }
                            stats.ProcessWeatherEvents(uow);
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

            sw.Stop();
            ms = sw.ElapsedMilliseconds;
            Trace.TraceInformation("PikAlert Worker Role Elapsed MS : " + ms.ToString());
        }

        private async Task<MAWOutput> AddMAWForSite(PA_Site site, string precipCondition, String pavementCondition, String alertCondition)
        {
            PA_MAWAlerts mawAlert = await GetPikalert_MAWAlerts(site);

            //PA_MAWAlerts mawAlert = GetPikalert_MAWAlertsFromFile(precipCondition, pavementCondition, alertCondition);

            MAWOutput mawOutput = new MAWOutput();
            mawOutput.ActionCode = mawAlert.alert_action_code;
            mawOutput.AlertGenTime = DateTime.ParseExact(mawAlert.alert_gen_time, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
            mawOutput.AlertRequestTime = DateTime.ParseExact(mawAlert.alert_request_time, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
            mawOutput.AlertTime = DateTime.ParseExact(mawAlert.alert_time, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
            mawOutput.PavementCode = mawAlert.alert_code_pavement;
            mawOutput.PrecipitationCode = mawAlert.alert_code_precip;
            mawOutput.VisibilityCode = mawAlert.alert_code_visibility;

            return mawOutput;
        }

        /// <summary>
        /// Records each PikAlert field to SiteObservation table.
        /// Parses the string fields characterizing the weather and records a numeric flag in the
        /// correct column [icy, snow, wet, clear] indicating the resulting conclusion about the weather.
        /// Adds SiteObservation to database - except skipped if pavement is null or empty (bad data).
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="site"></param>
        /// <param name="databaseSite"></param>
        private static SiteObservation RecordSiteObservation(WeatherEventStatistics stats, PA_Site site, Site databaseSite)
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
                return null;
            }
            else
            {
                //Interpret the string text and classify the weather.
                WeatherCondition paveCond = stats.ParseWeatherField(siteObs.Pavement);
                //Save the category the condition was parsed as.                                       
                if (paveCond == WeatherCondition.Wet) siteObs.PaveValWet = 1;
                else if (paveCond == WeatherCondition.Snow) siteObs.PaveValSnow = 1;
                else if (paveCond == WeatherCondition.Ice) siteObs.PaveValIce = 1;
                else siteObs.PaveValClear = 1;
                //Interpret the string text and classify the weather.
                WeatherCondition precipCond = stats.ParseWeatherField(siteObs.Precipitation);
                if (precipCond == WeatherCondition.Wet) siteObs.PrecipValWet = 1;
                else if (precipCond == WeatherCondition.Snow) siteObs.PrecipValSnow = 1;
                else if (precipCond == WeatherCondition.Ice) siteObs.PrecipValIce = 1;
                else siteObs.PrecipValClear = 1;
                databaseSite.SiteObservations.Add(siteObs);

                //Add the observation to the statistics object so that we can ProcessWeatherEvents later.
                stats.AddToStatistics(siteObs);
            }

            return siteObs;
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

        private async Task<PA_DistrictAlerts> GetPikalert_DistrictAlertsFromFiles()
        {

            string text="";
                try{
            var fileStream = new FileStream("DemoJson"+ currentFileInteger.ToString() +".json", FileMode.Open, FileAccess.Read);
            using(var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
                
            }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            PA_DistrictAlerts alerts = JsonConvert.DeserializeObject<PA_DistrictAlerts>(text);
            

            //clean up date and times

            DateTime dtNowUtc = DateTime.UtcNow;

            string dateString = dtNowUtc.ToString("yyyyMMddHHmm");

            alerts.data_time = dateString;

            foreach (var district in alerts.districts)
            {
                foreach(var site in district.sites)
                {
                    foreach(var time in site.time_series)
                    {
                        time.time = dateString;
                    }
                }
            }

            currentFileInteger++;
            
            if (currentFileInteger > 10)
                currentFileInteger = 1;

            return alerts;
        }

        private PA_MAWAlerts GetPikalert_MAWAlertsFromFile(String precipCondition, String pavementCondition, String alertCondition)
        {
            PA_MAWAlerts mawAlert = new PA_MAWAlerts();
            DateTime dtNowUtc = DateTime.UtcNow;

            string dateString = dtNowUtc.ToString("yyyyMMddHHmm");

            mawAlert.alert_gen_time = dateString;
            mawAlert.alert_request_time = dateString;
            mawAlert.alert_time = dateString;

            if(precipCondition.ToLower().Equals("none"))
            {
                mawAlert.alert_code_precip = 0;
            }
            else if(precipCondition.ToLower().Equals("light snow"))
            {
                mawAlert.alert_code_precip = 7;
            }else if(precipCondition.ToLower().Equals("moderate snow"))
            {
                mawAlert.alert_code_precip = 8;
            }else if(precipCondition.ToLower().Equals("heavy snow"))
            {
                mawAlert.alert_code_precip = 9;
            }

            if(pavementCondition.ToLower().Equals("dry"))
            {
                mawAlert.alert_code_pavement = 0;
            }
            else if (pavementCondition.ToLower().Equals("wet"))
            {
                mawAlert.alert_code_pavement = 1;
            }
            else if (pavementCondition.ToLower().Equals("icy"))
            {
                mawAlert.alert_code_pavement = 4;
            }
            else if (pavementCondition.ToLower().Equals("slick, icy"))
            {
                mawAlert.alert_code_pavement = 5;
            }
            else if (pavementCondition.ToLower().Equals("slick, snowy"))
            {
                mawAlert.alert_code_pavement=3;
            }

            if(alertCondition.ToLower().Equals("alert"))
            {
                if (mawAlert.alert_code_precip == 9)
                {
                    mawAlert.alert_action_code = 3;
                }
                else if (mawAlert.alert_code_precip == 8)
                {
                    if (mawAlert.alert_code_pavement == 0)
                    {
                        mawAlert.alert_action_code = 1;
                    }
                    else if (mawAlert.alert_code_pavement == 5)
                    {
                        mawAlert.alert_action_code = 3;
                    }
                    else
                    {
                        mawAlert.alert_action_code = 2;
                    }
                }
                else if (mawAlert.alert_code_precip == 7)
                {
                    if (mawAlert.alert_code_pavement == 0)
                    {
                        mawAlert.alert_action_code = 1;
                    }
                    else if (mawAlert.alert_code_pavement == 5)
                    {
                        mawAlert.alert_action_code = 3;
                    }
                    else
                    {
                        mawAlert.alert_action_code = 2;
                    }
                }
                else if (mawAlert.alert_code_precip == 0)
                {
                    if(mawAlert.alert_code_pavement ==0)
                    {
                        mawAlert.alert_action_code = 0;
                    }
                    else if (mawAlert.alert_code_pavement == 1)
                    {
                        mawAlert.alert_action_code = 1;
                    }
                    else
                    {
                        mawAlert.alert_action_code = 2;
                    }
                }          
            }
            else
            {
                mawAlert.alert_action_code = 0;
                mawAlert.alert_code_visibility = 0;
                mawAlert.alert_code_pavement = 0;
                mawAlert.alert_code_precip = 0;
            }

            return mawAlert;
        }

        private async Task<PA_MAWAlerts> GetPikalert_MAWAlerts(PA_Site site)
        {
            //http://vm-pikalert.cloudapp.net:8080/maw_alerts_adhoc?path=/maw_alerts_adhoc&lat=42.3373572&lon=-82.99628586&state=minnesota
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("path", "/maw_alerts_adhoc");
                parameters.Add("lat", site.lat);
                parameters.Add("lon", site.lon);
                parameters.Add("state", "minnesota");

                return await GetFromWebService<PA_MAWAlerts>("/maw_alerts_adhoc", parameters);
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
