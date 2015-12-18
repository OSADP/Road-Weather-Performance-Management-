using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CITOMobileCommon.Cloud;
using CITOMobileCommon.Models;

namespace CITOMobileCommon
{
     
    public class MotoristAlertsService
    {
        private CITOCloudApi CloudApi;
        private String BaseURL;
        private CancellationTokenSource TaskCancelSource;
        public delegate void NewMotoristAlertDelegate(Models.MotoristAlerts newMotoristAlerts);
        public event NewMotoristAlertDelegate NewMotoristAlertEvent;

        public MotoristAlertsService (String baseUrl)
        {
            BaseURL = baseUrl;

        }

        public void StartService(CITOMobileCommon.Locations.LocationMonitor locationMonitor)
        {
            CloudApi = new CITOCloudApi(BaseURL);

            TaskCancelSource = new CancellationTokenSource();
            var BackgroundCancelToken = TaskCancelSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    // do some heavy work here

                    
                    Coordinates location = locationMonitor.LastKnownLocation();
                    if (location != null)
                    {
                        Models.MotoristAlerts alerts = await CloudApi.GetMotoristAlertsForLocation(location);
                        if (alerts != null)
                        {
                            if (NewMotoristAlertEvent != null)
                            {
                                NewMotoristAlertEvent(alerts);
                            }

                        }
                    }
                    
                    Task.Delay(5000).Wait();
                    if (BackgroundCancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                }
            }, BackgroundCancelToken);
        }

        public void StopService()
        {
            if(TaskCancelSource!=null)  
                TaskCancelSource.Cancel();
        }

    }
}
