using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CITOMobileCommon.Cloud;

using CITOMobileCommon.Models;
using Xamarin.Forms;
using CITOMobileCommon.VITAL.Responses;
using System.Threading;
using CITOMobileCommon.Device;
using CITOMobileCommon.VITAL.Commands;

namespace CITOMobileCommon.Locations
{
    public delegate void ActivityDelegate(string sender, bool connected, bool sending);
    public delegate void EventDelegate(string eventString);
    public class LocationMonitor
    {

        public event ActivityDelegate LocationActivityEvent;
        public event ActivityDelegate CloudActivityEvent;
        public event ActivityDelegate VitalActivityEvent;
        public event EventDelegate StatusEvent;
        private DateTime LastRWReceiveTime;

        private List<ExtendedProbe> ProbePointList;
        private ILocationProvider LocProvider;
        private IDeviceInfo DeviceInfo;

        private bool isInTrip;
        private bool isConnectedToGps;
        private bool isConnectedToVital;
        private bool isConnectedToCloud;
        private bool uploadData;

        private Coordinates LastCoordinates;
        private CITOCloudApi CloudApi;
        private String BaseURL;
        private VITAL.VitalDevice CurrentVitalDevice;
        private VITAL.Responses.RoadWeatherStreamResponse LastRoadWeatherStreamResponse;
        private Object gpsLock = new Object();
        private Object vitalLock = new Object();
        public bool IsInTrip { get { return isInTrip; } }

        public CancellationTokenSource TaskCancelSource { get; private set; }

        public LocationMonitor(String baseUrl)
        {
            BaseURL = baseUrl;

            CloudApi = new CITOCloudApi(BaseURL);

            DeviceInfo = DependencyService.Get<IDeviceInfo>();

            LocProvider = DependencyService.Get<ILocationProvider>();
            

            ProbePointList = new List<ExtendedProbe>();

            LastCoordinates = null;
            isInTrip = false;
            uploadData = true;

        }

        public void StartMonitoringLocation(bool DisableActivityMonitoring)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                LocProvider.LocationChanged += LocProvider_LocationChanged;
                LocProvider.StartMonitoring();
            });
            isInTrip = true;

            LastRoadWeatherStreamResponse = null;

            TaskCancelSource = new CancellationTokenSource();
            var BackgroundCancelToken = TaskCancelSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    // do some heavy work here
                    //TODO Combine latest GPS and VITAL
                    try {
                        ExtendedProbe ppData = CombineVitalAndGPS();
                        if (ppData != null)
                        {
                            updateAddOnStatus("GPS", true);

                            await StoreExtendedProbe(ppData);

                        }
                        else
                        {
                            updateAddOnStatus("GPS", false);
                        }

                        Task.Delay(1000).Wait();
                        if (BackgroundCancelToken.IsCancellationRequested)
                        {
                            updateAddOnStatus("GPS", false);
                            break;
                        }
                    }catch(Exception ex)
                    {
                        String e = ex.Message;
                    }

                }
            }, BackgroundCancelToken);

            updateAddOnStatus("GPS", false);
            updateAddOnStatus("Cloud", false);
            updateAddOnStatus("VITAL", false);
        }

        public void AddVitalInformation(VITAL.VitalDevice vitalDevice)
        {
            CurrentVitalDevice = vitalDevice;
            if (CurrentVitalDevice != null)
            {
                
                CurrentVitalDevice.MessageReceived += VitalDevice_MessageReceived;
                CurrentVitalDevice.Disconnected += CurrentVitalDevice_Disconnected;
                if (StatusEvent != null)
                    StatusEvent("Sending Get TripInfo");

                CurrentVitalDevice.SendCmd(new GetTripInfoCommand());
            }
        }

        public void DisconnectCurrentVitalDevice()
        {
            if (CurrentVitalDevice != null)
            {
                if (StatusEvent != null)
                    StatusEvent("Disconnecting VITAL");

                CurrentVitalDevice.MessageReceived -= VitalDevice_MessageReceived;
                CurrentVitalDevice.Disconnected -= CurrentVitalDevice_Disconnected;
                CurrentVitalDevice.Disconnect();
                CurrentVitalDevice = null;
            }
            updateAddOnStatus("VITAL", false);
        }

        private void CurrentVitalDevice_Disconnected(object sender, EventArgs e)
        {
            DisconnectCurrentVitalDevice();
        }

        private ExtendedProbe CombineVitalAndGPS()
        {
            if(LastCoordinates!=null)
            {
                if ((DateTime.UtcNow - LastCoordinates.DateTime_UTC).TotalSeconds > 5.0)
                {
                    lock(gpsLock)
                    {
                        LastCoordinates = null;
                    }
                }
            }

            if (LastCoordinates != null)
            {
                
                ExtendedProbe ppData = new ExtendedProbe
                { //from VITAL
                    AirTemperature = 0,
                    AtmosphericPressure = 0,
                    HeadlightStatus = "Off",
                    LeftFrontWheelSpeed = 0,
                    LeftRearWheelSpeed = 0,
                    RightFrontWheelSpeed = 0,
                    RightRearWheelSpeed = 0,
                    Speed = 0,
                    SteeringWheelAngle = 0,
                    WiperStatus = "Off",
                    
                };
            
                lock (gpsLock)
                {

                    ppData.NomadicDeviceId = DeviceInfo.DeviceId();
                    ppData.DateGenerated = DateTime.UtcNow;
                    ppData.GpsElevation = LastCoordinates.Altitude;
                    ppData.GpsHeading = LastCoordinates.Heading;
                    ppData.GpsLatitude = LastCoordinates.Latitude;
                    ppData.GpsLongitude = LastCoordinates.Longitude;
                    ppData.GpsSpeed = LastCoordinates.Speed;
                    ppData.CVQueuedStatus = false;
                }


                if (LastRoadWeatherStreamResponse != null)
                {
                    LastRWReceiveTime = DateTime.Now;
                    lock (vitalLock)
                    {
                        ppData.AirTemperature = LastRoadWeatherStreamResponse.AmbientTemperature;
                        ppData.AtmosphericPressure = LastRoadWeatherStreamResponse.BarametricPressure;
                        if (LastRoadWeatherStreamResponse.HeadlightStatus > 0)
                            ppData.HeadlightStatus = "On";
                        else
                            ppData.HeadlightStatus = "Off";
                        ppData.LeftFrontWheelSpeed = LastRoadWeatherStreamResponse.FrontLeftWheelSpeed;
                        ppData.LeftRearWheelSpeed = LastRoadWeatherStreamResponse.RearLeftWheelSpeed;
                        ppData.RightFrontWheelSpeed = LastRoadWeatherStreamResponse.FrontRightWheelSpeed;
                        ppData.RightRearWheelSpeed = LastRoadWeatherStreamResponse.RearRightWheelSpeed;
                        ppData.Speed = LastRoadWeatherStreamResponse.Speed;
                        if (LastRoadWeatherStreamResponse.WiperStatus > 0)
                            ppData.WiperStatus = "On";
                        else
                            ppData.WiperStatus = "Off";

                        LastRoadWeatherStreamResponse = null;
                    }
                }
                else
                {
                    updateAddOnStatus("VITAL", false);

                    if (LastRWReceiveTime != DateTime.MinValue)
                    {
                        if ((DateTime.Now - LastRWReceiveTime).TotalSeconds > 30)
                        {
                            StopTrip();

                        }
                    }
                }

                return ppData;
                
            }

            return null;
        }

        private void VitalDevice_MessageReceived(object sender, VITAL.Responses.VitalResponse e)
        {
            if (e.ResponseType == VITAL.Responses.VitalResponseType.RoadWeatherStream)
            {

                Task.Run(() =>
                 {
                     lock (vitalLock)
                     {
                         updateAddOnStatus("VITAL", true);



                         LastRoadWeatherStreamResponse = (RoadWeatherStreamResponse)e;
                     }
                 });


            }
            else if (e.ResponseType == VITAL.Responses.VitalResponseType.TripInfoResponse)
            {
                TripInfoResponse tir = (TripInfoResponse)e;
                if (tir.IsOnTrip)
                {
                    //Turn On
                    uploadData = true;
                    StartMonitoringLocation(true);
                    if (StatusEvent != null)
                        StatusEvent("In Trip, Requesting RW Stream");
                    CurrentVitalDevice.SendCmd(new RoadWeatherStreamCommand(true));
                }
                else
                {
                    //Turn Off
                    if (StatusEvent != null)
                        StatusEvent("No In Trip");
                    StopTrip();
                }
            }
            else if (e.ResponseType == VITAL.Responses.VitalResponseType.RealTimeEvent)
            {
                RealTimeEventResponse rte = (RealTimeEventResponse)e;

                if (rte.Code == 161)
                {
                    //Turn On
                    uploadData = true;

                    StartMonitoringLocation(true);

                    if (StatusEvent != null)
                        StatusEvent("TripStart, Requesting RW Stream");
                    CurrentVitalDevice.SendCmd(new RoadWeatherStreamCommand(true));
                }
                else if (rte.Code == 162)
                {
                    //Turn Off
                    StopTrip();
                }
            }
        }

        private void StopTrip()
        {
            uploadData = false;
            StopMonitoringLocation();
        }

        public void StopMonitoringLocation()
        {
            try
            {
                TaskCancelSource.Cancel();

                if (isInTrip)
                {
                    isInTrip = false;
                    LocProvider.StopMonitoring();
                    LocProvider.LocationChanged -= LocProvider_LocationChanged;
                }

                updateAddOnStatus("Cloud", false);
                updateAddOnStatus("GPS", false);
                updateAddOnStatus("VITAL", false);
            }
            catch (Exception ex)
            {

            }
        }

        public Coordinates LastKnownLocation()
        {
            return LastCoordinates;
        }

        void LocProvider_LocationChanged(object sender, Coordinates e)
        {
            lock (gpsLock)
            {
                LastCoordinates = e;
            }
        }

        private void updateAddOnStatus(string addOnName, bool connected)
        {
            if(addOnName == "Cloud")
            {
                if (isConnectedToCloud != connected)
                {
                    isConnectedToCloud = connected;

                    if (CloudActivityEvent != null)
                        CloudActivityEvent("Cloud", isConnectedToCloud, isConnectedToCloud);
                }
            }
            else if(addOnName == "GPS")
            {
                if(isConnectedToGps!= connected)
                {
                    isConnectedToGps = connected;

                    if (LocationActivityEvent != null)
                        LocationActivityEvent("GPS", isConnectedToGps, isConnectedToGps);
                }
            }
            else if(addOnName == "VITAL")
            {
                if(isConnectedToVital!=connected)
                {
                    isConnectedToVital = connected;
                    if (VitalActivityEvent != null)
                        VitalActivityEvent("VITAL", isConnectedToVital, isConnectedToVital);
                }
            }
        }

        private async Task StoreExtendedProbe(ExtendedProbe ppData)
        {
            if (uploadData)
            {
                //TODO Get latest VITAL data and merge with coordinate data
                updateAddOnStatus("Cloud", true);

                ProbePointList.Add(ppData);

                if (ProbePointList.Count >= 10 && LocProvider.CanReachWebService())
                {
                    List<ExtendedProbe> probePointsToSend = new List<ExtendedProbe>(ProbePointList);

                    ProbePointList.Clear();

                    CITOCloudApi.PostResult postResult = await CloudApi.UploadExtendedProbePoints(probePointsToSend);
                }
            }
            else
            {
                updateAddOnStatus("Cloud", false);
            }

        }

        public bool IsConnectedToVital()
        {
            if (CurrentVitalDevice != null)
                return CurrentVitalDevice.IsConnected;
            else
                return false;
        }
    }
}
