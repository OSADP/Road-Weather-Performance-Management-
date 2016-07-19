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

namespace InfloWebRole.Controllers
{
    public class MotoristAlertController : ApiController
    {
        private static string strDatabaseConnectionString;
        //private static IUnitOfWork srInfloDbContext;

        private static string osmMapConnectionString;

        static MotoristAlertController()
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

        public async Task<MotoristAlertModel> Get(double latitude, double longitude, double heading)
        {
            Trace.TraceInformation("[TRACE] Entering MotoristAlertController::Get...");
            MotoristAlertModel m = new MotoristAlertModel();


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

                        RoadSegment roadSegment = rsMapper.GetNearestRoadSegment(latitude, longitude, heading);
                        if (roadSegment == null)
                        {
                            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                            {
                                Content = new StringContent(string.Format("No road segment found with Lat = {0}, Long = {1}, Heading = {2}", latitude, longitude, heading)),
                                ReasonPhrase = "GetNearestRoadSegment Not Found"
                            };
                            throw new HttpResponseException(resp);
                        }

                        double mileMarker = roadSegment.MileMarker;//157.2;
                        string RoadwayID = roadSegment.RoadwayID;//"1000";
                        bool increasingDir = roadSegment.IsMileMarkersIncreasing;
                        //Query speedHarm table for alerts
                        var spdHrmAlerts = srInfloDbContext.TMEOutput_SPDHARMMessage_CVs.Where(s => s.RoadwayId == RoadwayID);
                        if (increasingDir)
                        {
                            spdHrmAlerts = spdHrmAlerts.Where(s => s.BeginMM <= mileMarker && s.EndMM >= mileMarker);
                        }
                        else
                        {   //if decreasing direction
                            spdHrmAlerts = spdHrmAlerts.Where(s => s.BeginMM >= mileMarker && s.EndMM <= mileMarker);
                        }
                        var spdHrmAlert = spdHrmAlerts.OrderByDescending(d => d.DateGenerated).FirstOrDefault();
                        if (spdHrmAlert != null)
                        {
                            MotoristAlertModel.SpdHarmAlert a = new MotoristAlertModel.SpdHarmAlert();
                            a.DateGenerated = spdHrmAlert.DateGenerated;
                            a.RoadwayId = spdHrmAlert.RoadwayId;
                            a.RecommendedSpeed = spdHrmAlert.RecommendedSpeed;
                            a.BeginMM = spdHrmAlert.BeginMM;
                            a.EndMM = (double)spdHrmAlert.EndMM;
                            a.Justification = spdHrmAlert.Justification;
                            a.ValidityDuration = spdHrmAlert.ValidityDuration;
                            m.SpdHarm.Add(a);
                        }

                        //Query qWarn table for alerts

                        var qWarnlerts = srInfloDbContext.TMEOutput_QWARNMessage_CVs.Where(s => s.RoadwayID == RoadwayID)
                          .OrderByDescending(d => d.DateGenerated).FirstOrDefault();
                        if (qWarnlerts != null)
                        {
                            double dist = Math.Abs(qWarnlerts.BOQMMLocation - mileMarker);
                            if (dist <= 1)
                            {
                                MotoristAlertModel.QWarnAlert a = new MotoristAlertModel.QWarnAlert();
                                a.DateGenerated = qWarnlerts.DateGenerated;
                                a.RoadwayId = qWarnlerts.RoadwayID;
                                a.BOQMMLocation = qWarnlerts.BOQMMLocation;
                                a.FOQMMLocation = qWarnlerts.FOQMMLocation;
                                a.SpeedInQueue = (int)qWarnlerts.SpeedInQueue;
                                a.RateOfQueueGrowth = (double)qWarnlerts.RateOfQueueGrowth;
                                a.ValidityDuration = qWarnlerts.ValidityDuration;
                                a.DistanceToQueue = dist;
                                m.QWarn.Add(a);
                            }
                        }

                        //m.Pik.AddRange(GetPikAlertMAW(latitude, longitude));
                        //give me everything from .25 before me, and .5 miles ahead of me
                        double topBound = 0;
                        double bottomBound = 0;

                        if (increasingDir)
                        {
                            topBound = mileMarker + 0.5;
                            bottomBound = mileMarker - 0.25;
                        }
                        else
                        {
                            topBound = mileMarker + 0.25;
                            bottomBound = mileMarker - 0.5;
                        }

                        Trace.WriteLine("Searching for sites between mile marker " + topBound.ToString() + " and " + bottomBound.ToString());

                        var qSites = srInfloDbContext.Sites.Where(s => (s.MileMarker >= bottomBound) && (s.MileMarker <= topBound)).OrderBy(a => a.MileMarker).FirstOrDefault();

                        if (qSites != null)
                        {
                            Trace.WriteLine("Searching for mawAlert for site " + qSites.SiteIdName + " at mile marker " + qSites.MileMarker.ToString());
                            var mawAlert = srInfloDbContext.MAWOutputs.Where(a => a.SiteId == qSites.Id).OrderByDescending(a=>a.AlertGenTime).FirstOrDefault();
                            if (mawAlert != null)
                            {
                                Trace.WriteLine("Found mawAlert for site " + qSites.SiteIdName + " time " + mawAlert.AlertGenTime.ToShortTimeString() + " date " + mawAlert.AlertGenTime.ToShortDateString() + " action code " + mawAlert.ActionCode);
                                MotoristAlertModel.PikalertMAW maw = new MotoristAlertModel.PikalertMAW();
                                maw.AlertAction = MAWAlertCodeConverter.GetActionTextFromCode(mawAlert.ActionCode);
                                maw.AlertActionCode = mawAlert.ActionCode;
                                maw.AlertGenerationTime = mawAlert.AlertGenTime;
                                maw.AlertTime = mawAlert.AlertTime;
                                maw.DateGenerated = mawAlert.AlertRequestTime;
                                maw.MileMarker = qSites.MileMarker.Value;
                                maw.PavementAlert = MAWAlertCodeConverter.GetPavementAlertTextFromCode(mawAlert.PavementCode);
                                maw.PavementAlertCode = mawAlert.PavementCode;
                                maw.PrecipAlert = MAWAlertCodeConverter.GetPrecipitationAlertTextFromCode(mawAlert.PrecipitationCode);
                                maw.PrecipAlertCode = mawAlert.PrecipitationCode;
                                maw.RoadwayId = roadSegment.RoadwayID;
                                maw.VisibilityAlert = MAWAlertCodeConverter.GetVisibilityAlertTextFromCode(mawAlert.VisibilityCode);
                                maw.VisibilityAlertCode = mawAlert.VisibilityCode;

                                m.Pik.Add(maw);
                            }
                        }

                    }
                    catch (HttpResponseException ex)
                    { throw ex; }
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

                    //Return data
                    return m;

                }
            }
        }



        private List<MotoristAlertModel.PikalertMAW> GetPikAlertMAW(double latitude, double longitude, RoadSegment roadSegment)
        {
            List<MotoristAlertModel.PikalertMAW> maws = new List<MotoristAlertModel.PikalertMAW>();
            //Query PikAlert for alerts
            var client = new RestClient("http://vm-pikalert.cloudapp.net:8080");

            var request = new RestRequest("/maw_alerts_adhoc", Method.GET);
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("lat", latitude);
            request.AddParameter("lon", longitude);
            request.AddParameter("state", "minnesota");

            IRestResponse response = client.Execute(request);
            response.ContentType = "application/json";

            if (response != null)
            {
                String jsonRep = response.Content;


                MAWOutputModel mawOutput = Newtonsoft.Json.JsonConvert.DeserializeObject<MAWOutputModel>(jsonRep);

                if (mawOutput != null)
                {

                    MotoristAlertModel.PikalertMAW mawAlert = new MotoristAlertModel.PikalertMAW();
                    mawAlert.AlertGenerationTime = DateTime.ParseExact(mawOutput.alert_gen_time, "yyyyMMddHHmmss", null);
                    mawAlert.AlertTime = DateTime.ParseExact(mawOutput.alert_time, "yyyyMMddHHmmss", null);
                    mawAlert.DateGenerated = DateTime.UtcNow;
                    mawAlert.MileMarker = roadSegment.MileMarker;
                    mawAlert.PavementAlert = MAWAlertCodeConverter.GetPavementAlertTextFromCode(mawOutput.alert_code_pavement);
                    mawAlert.PavementAlertCode = mawOutput.alert_code_pavement;
                    mawAlert.PrecipAlert = MAWAlertCodeConverter.GetPrecipitationAlertTextFromCode(mawOutput.alert_code_precip);
                    mawAlert.PrecipAlertCode = mawOutput.alert_code_precip;
                    mawAlert.RoadwayId = roadSegment.RoadwayID;
                    mawAlert.VisibilityAlert = MAWAlertCodeConverter.GetVisibilityAlertTextFromCode(mawOutput.alert_code_visibility);
                    mawAlert.VisibilityAlertCode = mawOutput.alert_code_visibility;
                    mawAlert.AlertAction = MAWAlertCodeConverter.GetActionTextFromCode(mawOutput.alert_action_code);
                    mawAlert.AlertActionCode = mawOutput.alert_action_code;
                    maws.Add(mawAlert);
                }
            }

            return maws;
        }

    }
}
