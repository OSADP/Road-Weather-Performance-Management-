using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
//using Microsoft.WindowsAzure.StorageClient;
using Ninject;
using InfloCommon;
using InfloCommon.Repositories;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Xml;

namespace PikalertDataWorkerRole
{
    public class DetectorDataWorker : BaseProcWorker
    {
        //Get new data from http://www.dot.state.mn.us/tmc/trafficinfo/developers.html
        //Real-Time Detector Data ( http://data.dot.state.mn.us/iris_xml/det_sample.xml.gz )
        //This file is in XML format and is updated every 30 seconds. It contains volume, occupancy, speed and flow data for each detector in the Twin Cities Metro area.
        string BaseURL = "http://data.dot.state.mn.us/iris_xml/det_sample.xml.gz";
        private HttpClient HttpClient;

        private void TestLoopTime(IUnitOfWork uow)
        {
           

            DateTime currLocalTime = DateTime.Now;
            DateTime DateReceived = TimeZoneInfo.ConvertTimeToUtc(currLocalTime, TimeZoneInfo.Local);


            TME_TSSData_Input input = new TME_TSSData_Input();

            input.RoadwayId = "Test";//should match input config file
            input.DZId = DateReceived.ToString(); //make unique
            input.StartInterval = 0;
            input.EndInterval = 0;
            input.EndTime = DateReceived;
            input.BeginTime = DateReceived.AddSeconds(-20);
            input.IntervalLength = 30;
            input.DateReceived = DateReceived;
            input.Volume = 10;
            input.AvgSpeed = (short)DateReceived.Minute;
            input.Occupancy = DateReceived.Second;
            input.Queued = false;
            input.Congested = false;
           

            uow.TME_TSSData_Inputs.Add(input);
            uow.Commit();
        }
        public override async void PerformWork()
        {
            Trace.TraceInformation("DetectorDataWorker called");
            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            HttpClient.Timeout = TimeSpan.FromSeconds(15);

            try
            {
                Configuration_INFLOThresholds config = uow.Configuration_INFLOThresholds.FirstOrDefault();
                if (config == null)
                {
                    throw new Exception("Configuration_INFLOThresholds null.");
                }

                traffic_sample sensorData = await GetDetectorData();
                if (sensorData != null)
                {
                    
                    //Add data to database.
                    foreach (sample s in sensorData.samples)
                    {
                        //Lookup roadway from detector station table
                        var station = uow.Configuration_TSSDetectionZones.Where(d => d.DZId == s.sensor).FirstOrDefault();
                        if (station != null)  //Only add if the station is in our list.
                        {
                            //TME_TSSData_Input's unique key is a compound key of:[DZId] ASC,[Volume] ASC,[Occupancy] ASC,[AvgSpeed] ASC
                            //which means that new data cannot be added that may have the same vol/occ/speed, which is reasonably likely 30seconds later.
                            //11/20 FIXED: TME_TSSData_Input updated to use an Id integer as primary key!

                            //Volume, occupancy, and speed are not allowed to be null in the database.
                            if (s.speed != null && s.flow != null && s.occ != null)
                            {
                               
                                short speed = (short)s.speed;
                                short vol = (short)s.flow;
                                float occ = (float)s.occ;
                                TME_TSSData_Input  input = new TME_TSSData_Input();
                                  
                                input.AvgSpeed = speed;
                                input.Volume = vol;
                                input.Occupancy = occ;
                                //Set Queued or Congested if speeds are below a certain threshold.
                                if (input.AvgSpeed <= config.QueuedSpeedThreshold)
                                {
                                    input.Queued = true;
                                }
                                else input.Queued = false;
                                if (input.AvgSpeed <= config.CongestedSpeedThreshold)
                                {
                                    input.Congested = true;
                                }
                                else input.Congested = false;

                                //Look up sensor
                                input.DZId = s.sensor;
                                input.DSId = null; //unused?//our zones and stations are 1-to-1
                                input.DateReceived = DateTime.UtcNow;

                                input.StartInterval = 0;
                                input.EndInterval = 0;
                                //EndTime is when the data was read or received by the system. The BeginTime is the EndTime minus
                                //the length of the data collection interval. In the case of Minnesota, the XML file that contains 
                                //the detector station data contains data for the last 30 seconds. So the BeginTime is equal to the EndTime – 30 
                                input.EndTime = input.DateReceived;
                                input.BeginTime = ((DateTime)input.DateReceived).AddSeconds(-30);
                                input.IntervalLength = config.TSSDataLoadingFrequency;

                                //DZStatus leave null
                                //DataType leave null

      
                                input.RoadwayId = station.RoadwayId;
                                
                                uow.TME_TSSData_Inputs.Add(input);
                               
                            }
                        }
                    }
                    uow.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string inner = "";
                if (ex.InnerException != null)
                    inner = ex.InnerException.Message + "     " + ex.InnerException.StackTrace;
                Trace.TraceError("Exception occurred when getting DetectorDataWorker data. " +
                               ex.Message + " " + ex.StackTrace + "     " + inner);
            }
        }


        private async Task<traffic_sample> GetDetectorData()
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                return await GetFromWebService<traffic_sample>("", parameters);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        protected async Task<traffic_sample> GetFromWebService<T>(String relativeUrl, Dictionary<string, object> parameters)
        {
            try
            {
                String url = BaseURL + relativeUrl;
                url = AddUrlParams(url, parameters);
                HttpResponseMessage response = await HttpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    //Content comes zipped. Unzip it.
                    Stream content = await response.Content.ReadAsStreamAsync();

                    string xml;
                    using (var decompress = new GZipStream(content, CompressionMode.Decompress))
                    using (var sr = new StreamReader(decompress))
                    {
                        xml = sr.ReadToEnd();
                    }
                    //Now we have xml. Parse it.

                    //Tried to use Deserialize, couldn't get the sample array to load.
                    //XmlSerializer serializer = new XmlSerializer(typeof(traffic_sample));
                    // traffic_sample sensorSamples = (traffic_sample)serializer.Deserialize(new StringReader(xml));

                    XmlDocument doc = new XmlDocument();

                    doc.LoadXml(xml);
                    traffic_sample sensorData = new traffic_sample();
                    XmlNode nodeRoot = doc.DocumentElement.SelectSingleNode("/traffic_sample");
                    if (nodeRoot.Attributes["time_stamp"] != null)
                    {
                        sensorData.time_stamp = nodeRoot.Attributes["time_stamp"].InnerText;
                    }
                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        string text = node.InnerText; //or loop through its children as well
                        sample s = new sample();
                        try
                        {
                            s.sensor = node.Attributes["sensor"].InnerText;
                            if (node.Attributes["flow"] != null && node.Attributes["flow"].InnerText != "UNKNOWN")
                            {
                                s.flow = Convert.ToInt16(node.Attributes["flow"].InnerText);
                            }
                            if (node.Attributes["speed"] != null && node.Attributes["speed"].InnerText != "UNKNOWN")
                            {
                                s.speed = Convert.ToInt16(node.Attributes["speed"].InnerText);
                            }
                            if (node.Attributes["occ"] != null && node.Attributes["occ"].InnerText != "UNKNOWN")
                            {
                                s.occ = Convert.ToDouble(node.Attributes["occ"].InnerText);
                            }
                            sensorData.samples.Add(s);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceWarning("Skipping Bad data recieved from detector xml in DetectorDataWorker.GetFromWebService: " + node.OuterXml);
                        }
                    }

                    return sensorData;
                }
                else
                {
                    String message = "Get error status code: " + response.StatusCode + " ; body: " + response.Content.ToString();
                    Trace.TraceError("Failure obtaining HttpResponseMessage in DetectorDataWorker.GetFromWebService: " + message);
                    throw new Exception(message);
                }
            }

            catch (Exception ex)
            {
                Trace.TraceError("Error in DetectorDataWorker.GetFromWebService: " + ex.Message + "  " + ex.StackTrace);
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

        [Serializable()]
        [XmlRoot()]
        public class traffic_sample
        {
            public traffic_sample()
            {
                samples = new List<sample>();
            }
            [XmlAttribute]
            public string time_stamp { get; set; }
            [XmlAttribute]
            public int period { get; set; }
            [XmlArray("traffic_sample")]
            [XmlArrayItem("sample", typeof(sample))]
            public List<sample> samples { get; set; }


        }


        [Serializable()]
        public class sample
        {
            [XmlAttribute]
            public string sensor { get; set; }
            [XmlAttribute]
            public short? flow { get; set; }
            [XmlAttribute]
            public short? speed { get; set; }
            [XmlAttribute]
            public double? occ { get; set; }
        }




    }


}
