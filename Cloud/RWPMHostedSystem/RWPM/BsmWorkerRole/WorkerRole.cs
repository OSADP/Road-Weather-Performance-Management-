using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using InfloCommon.Models;
using InfloCommon;
using InfloCommon.Repositories;
using RoadSegmentMapping;
using System.Data.Entity;
using Microsoft.WindowsAzure.Storage.Queue;

namespace BsmWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        /*
        * Static Variables
        */
        private static Microsoft.WindowsAzure.Storage.CloudStorageAccount srStorageAccount;

        private static Microsoft.WindowsAzure.Storage.Queue.CloudQueueClient srCloudQueueClient;
        private static Microsoft.WindowsAzure.Storage.Queue.CloudQueue srBsmQueue;
        private static string srBsmQueueName = "inbound-bsm-bundles";
        private static Microsoft.WindowsAzure.Storage.Queue.CloudQueue srProbeQueue;
        private static string srProbeQueueName = "inbound-probes";

        private static string srBsmTimeTableName = "BsmTimeTable";

        private static IUnitOfWork uow;
     //   InfloEntities dbcontext;
        private static string srLogStatisticalDataSettingKey = "Stats_LogStatisticalData";
        private static string srEndToEndLoggingThresholdSettingKey = "Stats_MinimalLoggedElapsedTime";

        private static string osmMapDbConnectionString;
        private RoadSegmentMapper rsMapper;

        public override void Run()
        {
            Trace.TraceInformation("BsmWorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            catch (Exception e)
            {
                string message = e.Message;
                string stackTrace = e.StackTrace;
                if (e.InnerException != null)
                {
                    message += e.InnerException.Message;
                    stackTrace += e.InnerException.StackTrace;
                }
                Trace.TraceError("Exception occurred BSM RunAsync.  \n{0}\n{1}",
                        message, stackTrace);
                throw e;
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.Changed += RoleEnvironment_Changed;

            Trace.TraceInformation("[TRACE] Entering BsmWorkerRole::OnStart()...");

            ServicePointManager.DefaultConnectionLimit = 12;  //maximum number of concurrent connections 

            // Load SQL Server Types for RoadSegmentMatter Distance functions.
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

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
                Trace.TraceInformation("[INFO] Retrieved StorageAccountConnectionString for BsmWorkerRole\n{0}",
                    strStorageAccountConnectionString);

                try
                {
                    srStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(strStorageAccountConnectionString);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Exception occurred when parsing storage account connection string\n{0}\n{1}",
                        strStorageAccountConnectionString, e.Message);
                    throw (e);
                }

                if (srStorageAccount != null)
                {
                    srCloudQueueClient = srStorageAccount.CreateCloudQueueClient();
                    srBsmQueue = srCloudQueueClient.GetQueueReference(srBsmQueueName);

                    try
                    {
                        if (srBsmQueue.CreateIfNotExists())
                        {
                            Trace.TraceInformation("Created Azure BSM queue '{0}'", srBsmQueueName);
                        }
                        else
                        {
                            Trace.TraceInformation("Got reference to existing BSM queue '{0}'", srBsmQueueName);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Exception occurred when creating queue for inbound BSM bundles\n{0}",
                            e.Message);

                        srBsmQueue = null;
                        throw (e);
                    }

                    BsmTimeTableLogger.Initialize(srStorageAccount, srBsmTimeTableName);

                    srProbeQueue = srCloudQueueClient.GetQueueReference(srProbeQueueName);

                    try
                    {
                        if (srProbeQueue.CreateIfNotExists())
                        {
                            Trace.TraceInformation("Created Azure Probe queue '{0}'", srProbeQueueName);
                        }
                        else
                        {
                            Trace.TraceInformation("Got reference to existing Probe queue '{0}'", srProbeQueueName);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Exception occurred when creating queue for inbound Probe bundles\n{0}",
                            e.Message);

                        srProbeQueue = null;
                        throw (e);
                    }
                }
            }

            string strDatabaseConnectionString =
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("InfloDatabaseConnectionString");

            if (strDatabaseConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve Inflo database connection string");
                throw new Exception("BSM Worker Role Configuration Failure: Unable to retrieve Inflo database connection string");
            }
            else if (strDatabaseConnectionString.Length <= 0)
            {
                Trace.TraceError("Inflo Database connection string empty");
                throw new Exception("BSM Worker Role Configuration Failure: Inflo Database connection string empty");
            }
            else  //connect to the database
            {
                Trace.TraceInformation("[INFO] Retrieved InfloDatabaseConnectionString for BsmWorkerRole\n{0}",
                    strDatabaseConnectionString);

                uow = new UnitOfWork(strDatabaseConnectionString);
        //        this.dbcontext = new InfloEntities(strDatabaseConnectionString);
            }

            osmMapDbConnectionString =
      Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("OsmMapModelDbConnectionString");

            if (osmMapDbConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve OsmMapDbConnectionString");
                throw new Exception("BSM Worker Role Configuration Failure: Unable to retrieve OsmMapDbConnectionString");
            }
            else if (osmMapDbConnectionString.Length <= 0)
            {
                Trace.TraceError("OsmMapDbConnectionString empty");
                throw new Exception("BSM Worker Role Configuration Failure: OsmMapDbConnectionString empty");
            }
            rsMapper = new RoadSegmentMapper(osmMapDbConnectionString);

            BsmTimeTableLogger.Enabled = IsStatisticalLoggingEnabled();
            BsmTimeTableLogger.MinimalLoggedElapsedTime = GetMinimalLoggedElapsedTime();

            Trace.TraceInformation("[TRACE] Exiting BsmWorkerRole::OnStart()...");
            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("BsmWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("BsmWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {

            Trace.TraceInformation("[TRACE] Entering BsmWorkerRole::Run()...");

            if (srBsmQueue == null)
            {
                Trace.TraceError("ERROR: Unable to run BsmWorkerRole-- Inbound BSM queue does not exist");
                Trace.TraceInformation("[TRACE] Exiting BsmWorkerRole::Run()...");
                return;
            }

            const int minPollWaitPeriod_ms = 10;
            const int maxPollWaitPeriod_ms = 1000;
            const int maxQueueMessagePollCount = 32; //32 is the MAX allowed
            const int messageLeaseTime = 2;
            int currPollWaitPeriod_ms = maxPollWaitPeriod_ms;

            while (!cancellationToken.IsCancellationRequested)
            {
                //Disable reading from Bsm message queue, no sense wasting azure credits - depricated until use of infloDsrcToolkit logic is recreated.
                var rBsmMessages = new List<Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage>();
                //var rBsmMessages = srBsmQueue.GetMessages(maxQueueMessagePollCount, new TimeSpan(0, 0, 0, messageLeaseTime * maxQueueMessagePollCount));
                var probeMessages = srProbeQueue.GetMessages(maxQueueMessagePollCount, new TimeSpan(0, 0, 0, messageLeaseTime * maxQueueMessagePollCount));
                List<CloudQueueMessage> addedMessages = new List<CloudQueueMessage>();
                //Stopwatch commit = new Stopwatch();
                //Stopwatch rw = new Stopwatch();
                //Stopwatch fullmessage = new Stopwatch();
               
                //Stopwatch cvadd = new Stopwatch();
                //Stopwatch add = new Stopwatch();
                if (rBsmMessages.Count() > 0)
                {
                    List<TME_CVData_Input> rCvDataEntriesToAdd = new List<TME_CVData_Input>();

                    if (currPollWaitPeriod_ms > minPollWaitPeriod_ms)
                    {
                        // Get the queue message count
                        srBsmQueue.FetchAttributes();
                        int? messageCount = srBsmQueue.ApproximateMessageCount;

                        Trace.WriteLine(String.Format("Processing {0} new queue messages...",
                            messageCount));
                    }

                    BsmTimeTableLogger.StartNewLogEntry(srBsmQueue, rBsmMessages);

                    foreach (var rMessage in rBsmMessages)
                    {
                        string strMessageJSON = rMessage.AsString;

                        try
                        {
                            /*
                             * Extract raw bsm data from queue message
                             */
                            BsmBundle rBsmBundle = Newtonsoft.Json.JsonConvert.DeserializeObject<BsmBundle>(strMessageJSON);

                            foreach (var bsm in rBsmBundle.payload)
                            {
                                TME_CVData_Input rCvData = null;
                                try
                                {
                                    DateTimeOffset extractStartTime = DateTimeOffset.Now;
                                    //Validate conversion and BSM inside 'GenerateFromBsmMessage'.  Throw exception with errors.
                                    rCvData = GenerateCvDataFromBsmMessage(bsm);

                                    if ((DateTimeOffset.Now - extractStartTime).TotalSeconds > 0.5)
                                        Trace.TraceWarning("Took {0} seconds to extract BSM Data", (DateTimeOffset.Now - extractStartTime).TotalSeconds);
                                }
                                catch (Exception e)
                                {
                                    Trace.TraceWarning("Exception occurred when extracting CV Data from BSM Payload\n{0}",
                                        e.Message);
                                }

                                //If conversion was good, add to DbSet.
                                if (rCvData != null)
                                {
                                    rCvDataEntriesToAdd.Add(rCvData);
                                }
                            }
                        }
                        catch (Newtonsoft.Json.JsonSerializationException e)
                        {
                            Trace.TraceError("Exception when deserializing queue item. Message: '{0}'",
                                e.Message);
                        }
                        finally
                        {
                            currPollWaitPeriod_ms = minPollWaitPeriod_ms;
                        }
                    }

                    BsmTimeTableLogger.SetDeserializationComplete();

                    foreach (var rMessage in rBsmMessages)
                    {
                        try
                        {
                            srBsmQueue.DeleteMessage(rMessage);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Exception occurred when deleting message from queue.  Did the processing of the BSM take longer then the message's lease time?\n{0}",
                                    e.Message);
                        }
                    }

                    BsmTimeTableLogger.SetQueueDeleteComplete();

                    //Save changes to DB.
                    if (rCvDataEntriesToAdd.Count() > 0)
                    {
                        foreach (var rCvData in rCvDataEntriesToAdd)
                        {
                            try
                            {
                                DateTimeOffset dbSetAddStartTime = DateTimeOffset.Now;
                                /*
                                 * This use to take forever when the dbSet got large.
                                 * This was fixed by adding "base.Configuration.AutoDetectChangesEnabled = false;"
                                 * to the dbContext constructor.
                                 */
                                uow.TME_CVData_Inputs.Add(rCvData);
                                if ((DateTimeOffset.Now - dbSetAddStartTime).TotalSeconds > 0.5)
                                    Trace.TraceWarning("Took {0} seconds to add CvData to DbSet", (DateTimeOffset.Now - dbSetAddStartTime).TotalSeconds);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("Exception occurred when adding CV Data to DbSet\n{0}",
                                    e.Message);
                            }
                        }


                        try
                        {
                            uow.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Exception occurred when saving changes to Db\n{0}", e);
                        }

                        rCvDataEntriesToAdd.ForEach(x => uow.Entry<TME_CVData_Input>(x).State = System.Data.Entity.EntityState.Detached);
                    }

                    BsmTimeTableLogger.SubmitLogEntry(rCvDataEntriesToAdd.Count());
                }
                else if (probeMessages.Count() > 0)
                {
                    List<TME_CVData_Input> rCvDataEntriesToAdd = new List<TME_CVData_Input>();

                    //KG commented out, not sure why this was done this way above?
                    //if (currPollWaitPeriod_ms > minPollWaitPeriod_ms)
                    //{
                    //    // Get the queue message count
                    //    srProbeQueue.FetchAttributes();
                    //    int? messageCount = srProbeQueue.ApproximateMessageCount;

                    //    Trace.WriteLine(String.Format("Processing {0} new queue messages...",
                    //        messageCount));
                    //}
                    Trace.WriteLine(String.Format("Processing {0} new queue messages...",
                           probeMessages.Count()));
                    foreach (var rMessage in probeMessages)
                    {
                        if (rMessage.DequeueCount <= 3)//Don't process bad messages forever
                        {
                            string strMessageJSON = rMessage.AsString;

                            try
                            {
                                //fullmessage.Reset();
                                //fullmessage.Start();
                                //   firstpart.Reset();
                                //    firstpart.Start();
                                // This would explicitly remove all of the backslashes from your string
                                strMessageJSON = strMessageJSON.Replace(@"\""", @"""");
                                strMessageJSON = strMessageJSON.Replace(@"\\", @"\");
                                //Trim start " and end "
                                strMessageJSON = strMessageJSON.Trim(@"""".ToCharArray());
                                /*
                                 * Extract raw probe data from queue message
                                 */
                                RWProbeModel m = Newtonsoft.Json.JsonConvert.DeserializeObject<RWProbeModel>(strMessageJSON);
                                //firstpart.Stop();
                                if (m.IsBsm())//Record entry to TME_CVData_Input if data matches character.
                                {
                                    TME_CVData_Input cvInput = new TME_CVData_Input();

                                    //rw.Reset();
                                    //rw.Start();
                                    //Use Lat/Long to query the milemarker and roadwayId.
                                    //Translate Lat/Long/Heading into Roadway ID and MM.


                                    RoadSegment roadSegment = rsMapper.GetNearestRoadSegment(m.GpsLatitude, m.GpsLongitude, m.GpsHeading);
                                    //rw.Stop();

                                    if (roadSegment != null)
                                    {
                                        cvInput.MMLocation = roadSegment.MileMarker;
                                        cvInput.RoadwayId = roadSegment.RoadwayID;

                                        cvInput.NomadicDeviceID = m.NomadicDeviceId;
                                        cvInput.DateGenerated = m.DateGenerated;
                                        cvInput.Speed = m.GpsSpeed;
                                        cvInput.Heading = (short)m.GpsHeading;
                                        cvInput.Latitude = m.GpsLatitude;
                                        cvInput.Longitude = m.GpsLongitude;
                                        cvInput.CVQueuedState = m.CVQueuedStatus;
                                        cvInput.LateralAcceleration = m.LatAccel;
                                        cvInput.LongitudinalAcceleration = m.LongAccel;
                                        //cvadd.Reset();
                                        //cvadd.Start();
                                        uow.TME_CVData_Inputs.Add(cvInput);
                                     
                                     //   cvadd.Stop();
                                    }
                                    else
                                    {
                                        //Trucks not driving around on just our highway - it is quite frequent that we wont find a match.
                                       // Trace.TraceError(string.Format("RoadSegment from GetNearestRoadSegment null. Lat = {0}, Long = {1}, Heading = {2}, Date = {3}", m.GpsLatitude, m.GpsLongitude, m.GpsHeading,m.DateGenerated));
                                    }
                                }

                                //Always add it to the Probe table (regardless of IsBsm()).
                                RoadWeatherProbeInputs input = new RoadWeatherProbeInputs();

                                //Date Generated is the time that the phone created the data packet
                                input.DateGenerated = m.DateGenerated;
                                //Date Created is the time we are saving it to the database.

                                input.DateCreated = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
                                input.AirTemperature = m.AirTemperature;
                                input.AtmosphericPressure = m.AtmosphericPressure;
                                input.GpsElevation = m.GpsElevation;
                                input.GpsHeading = m.GpsHeading;
                                input.GpsLatitude = m.GpsLatitude;
                                input.GpsLongitude = m.GpsLongitude;
                                input.GpsSpeed = m.GpsSpeed;
                                if (m.HeadlightStatus != null)
                                {
                                    string s = m.HeadlightStatus.ToString().ToLower();
                                    if (s == "on" || s == "true" || s == "1")
                                    {
                                        input.HeadlightStatus = true;
                                    }
                                    else
                                    {
                                        input.HeadlightStatus = false;
                                    }
                                }

                                input.LeftFrontWheelSpeed = m.LeftFrontWheelSpeed;
                                input.LeftRearWheelSpeed = m.LeftRearWheelSpeed;
                                input.NomadicDeviceId = m.NomadicDeviceId;
                                input.RightFrontWheelSpeed = m.RightFrontWheelSpeed;
                                input.RightRearWheelSpeed = m.RightRearWheelSpeed;
                                input.Speed = m.Speed;
                                input.SteeringWheelAngle = m.SteeringWheelAngle;
                                if (m.WiperStatus != null)
                                {
                                    string s = m.WiperStatus.ToString().ToLower();
                                    if (s == "on" || s == "true" || s == "1")
                                    {
                                        input.WiperStatus = true;
                                    }
                                    else
                                    {
                                        input.WiperStatus = false;
                                    }
                                }

                               // add.Reset();
                               // add.Start();
                                uow.RoadWeatherProbeInputs.Add(input);
                                addedMessages.Add(rMessage);
                                
                               // add.Stop();
                               // fullmessage.Stop();
  

                               // Trace.TraceInformation("RoadSegmentMapper dur '{0}' Total msg dur '{0}' cvadd dur '{0}' rwadd dur '{0}'",
                                //     rw.ElapsedMilliseconds, fullmessage.ElapsedMilliseconds, cvadd.ElapsedMilliseconds, add.ElapsedMilliseconds);

                            }
                            catch (Newtonsoft.Json.JsonSerializationException e)
                            {
                                Trace.TraceError("Exception when deserializing queue item. Message: '{0}'",
                                    e.Message);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("Exception occurred when processing message from queue.MessageId:{0}\n{1}\n{2}", rMessage.Id,
                                        e.Message, e.StackTrace);
                            }
                            finally
                            {
                                currPollWaitPeriod_ms = minPollWaitPeriod_ms;
                            }
                        }
                        else//this is a bad message and has failed 3 times.
                        {
                            Trace.TraceError("RWProbe message has failed 3 times. Deleting. MessageId:{0}\n{1}\n{2}", rMessage.Id,
                                        rMessage.AsString, rMessage.InsertionTime);
                            srProbeQueue.DeleteMessage(rMessage);
                        }
                    }

                    uow.SaveChanges();
                    foreach (var rMessage in addedMessages)//delete the messages from teh queue that successfully processed.
                    {
                        try
                        {
                            srProbeQueue.DeleteMessage(rMessage);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Exception occurred when deleting message from queue.  Did the processing of the Probe take longer then the message's lease time?\n{0}",
                                    e.Message);
                        }
                    }
                }
                else //if(rMessages.Count() == 0)
                {
                 //   Trace.WriteLine(String.Format("Waiting {0}ms for new {1} or {2} queue items...",
                 //       currPollWaitPeriod_ms, srBsmQueueName, srProbeQueueName));

                    await Task.Delay(currPollWaitPeriod_ms);

                    // Gradually scale up the poll wait period until we reach the max (10,20,40,80,160,...1000)
                    currPollWaitPeriod_ms *= 2;
                    currPollWaitPeriod_ms = Math.Min(currPollWaitPeriod_ms, maxPollWaitPeriod_ms);
                }


            }
        }


        void RoleEnvironment_Changed(object sender, Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironmentChangedEventArgs e)
        {
            if (e.Changes.OfType<Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironmentConfigurationSettingChange>().Count() > 0)
            {
                BsmTimeTableLogger.Enabled = IsStatisticalLoggingEnabled();
                BsmTimeTableLogger.MinimalLoggedElapsedTime = GetMinimalLoggedElapsedTime();
            }
        }


        /*
         * Static Methods
         */
        public static TME_CVData_Input GenerateCvDataFromBsmMessage(BsmMessage bsm)
        {
            TME_CVData_Input results = new TME_CVData_Input();

            byte[] rawBsm;

            try
            {
                //Converted payload to raw bsm byte[] (ASN.1)
                rawBsm = Enumerable.Range(0, bsm.payload.Length)
                    .Where(i => i % 2 == 0)
                    .Select(i => Convert.ToByte(bsm.payload.Substring(i, 2), 16))
                    .ToArray();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Unable to parse bsm.payload to byte[]: {0}", bsm.payload), e);
            }

            //ExtractedBSM rExtractedBsm = new ExtractedBSM();

            ////Try to extract the BSM from the raw ASN.1 byte[]
            //if (!rExtractedBsm.loadFromASN(rawBsm))
            //{
            //    throw new Exception(string.Format("Error deserializing BSM from ASN.1: {0}",
            //        string.Concat(Array.ConvertAll(rawBsm, b => b.ToString("X2")))));
            //}

            //results.NomadicDeviceID = rExtractedBsm.getNomadicId();
            //results.DateGenerated = DateTime.Parse(bsm.time);
            //results.Speed = (short)rExtractedBsm.getSpeed() * 2.236936292054402;
            //results.Heading = (short)rExtractedBsm.getHeading();
            //results.Latitude = rExtractedBsm.getLatitude();
            //results.Longitude = rExtractedBsm.getLongitude();
            //results.MMLocation = rExtractedBsm.getMileMarker();
            //results.CVQueuedState = rExtractedBsm.getQueuedState();
            //results.CoefficientOfFriction = rExtractedBsm.getCoefOfFriction();
            //results.Temperature = (short)rExtractedBsm.getAirTemp();
            //results.RoadwayId = rExtractedBsm.getRoadwayId();
            //results.LateralAcceleration = rExtractedBsm.getLatAccel();
            //results.LongitudinalAcceleration = rExtractedBsm.getLongAccel();

            //return results;
            Trace.TraceWarning("BSM Data extracted deprecated. Do not use.");

            return null;
        }

        public static bool IsStatisticalLoggingEnabled()
        {
            bool results = false;

            string enabledString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting(srLogStatisticalDataSettingKey);
            if (enabledString != null)
            {
                try
                {
                    results = Convert.ToBoolean(enabledString);
                }
                catch (FormatException) { Trace.TraceWarning("Unable to parse setting key value to boolean: {0}-{1}", srLogStatisticalDataSettingKey, enabledString); }
            }

            return results;
        }

        public static int GetMinimalLoggedElapsedTime()
        {
            int results = 0;

            string enabledString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting(srEndToEndLoggingThresholdSettingKey);
            if (enabledString != null)
            {
                try
                {
                    results = Convert.ToInt32(enabledString);
                }
                catch (FormatException) { Trace.TraceWarning("Unable to parse setting key value to int: {0}-{1}", srLogStatisticalDataSettingKey, enabledString); }
            }

            return results;
        }
    }
}
