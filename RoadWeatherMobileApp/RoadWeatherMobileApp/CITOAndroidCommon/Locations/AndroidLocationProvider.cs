using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Locations;
using Xamarin.Forms;
using Application = Android.App.Application;


using CITOMobileCommon.Locations;
using CITOMobileCommon.Models;

using Android.Net;
using Android.Util;

[assembly: Xamarin.Forms.Dependency(typeof(CITOAndroidCommon.Locations.AndroidLocationProvider))]

namespace CITOAndroidCommon.Locations
{
    public class AndroidLocationProvider : Java.Lang.Object, ILocationProvider, ILocationListener
    {
        public event EventHandler<Coordinates> LocationChanged;
        private LocationManager LocMgr;

        public void StartMonitoring()
        {
            try
            {
                LocMgr = Application.Context.GetSystemService("location") as LocationManager;
                var locationCriteria = new Criteria();

                locationCriteria.Accuracy = Accuracy.Fine;
                locationCriteria.PowerRequirement = Power.NoRequirement;

                // get provider: GPS, Network, etc.
                var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);

                // Get an initial fix on location
                LocMgr.RequestLocationUpdates(locationProvider, 2000, 0, this);


            }
            catch (Exception ex)
            {
                // DebugLogger.Log(ex);
            }
        }
        public void StopMonitoring()
        {
            try
            {
                LocMgr.RemoveUpdates(this);
            }
            catch (Exception ex)
            {
                //DebugLogger.Log(ex);
            }
        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                if (LocationChanged != null)
                {
                    Coordinates coordinatesToReturn = new Coordinates
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Accuracy = location.Accuracy,
                        Altitude = location.Altitude,
                        Speed = location.Speed,
                        Heading = location.Bearing
                    };

                    String logTag = "ALP";
                    Log.Debug(logTag, String.Format("Latitude is {0}", location.Latitude));
                    Log.Debug(logTag, String.Format("Longitude is {0}", location.Longitude));
                    Log.Debug(logTag, String.Format("Altitude is {0}", location.Altitude));
                    Log.Debug(logTag, String.Format("Speed is {0}", location.Speed));
                    Log.Debug(logTag, String.Format("Accuracy is {0}", location.Accuracy));
                    Log.Debug(logTag, String.Format("Bearing is {0}", location.Bearing));

                    LocationChanged(this, coordinatesToReturn);
                }
            }
            catch (Exception ex)
            {
                //DebugLogger.Log(ex);
            }
        }

        public void OnProviderDisabled(string provider)
        {
            //DebugLogger.Log("OnProviderDisabled called for provider " + provider);
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //DebugLogger.Log("OnProviderEnabled called for provider " + provider);
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            try
            {
                string msg = string.Format("OnStatusChanged called: provider = {0}, status = {1}", provider, status);
                //DebugLogger.Log(msg);
            }
            catch (Exception ex)
            {
                //DebugLogger.Log(ex);
            }
        }

        public bool CanReachWebService()
        {
            var connectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Application.ConnectivityService);

            var activeConnection = connectivityManager.ActiveNetworkInfo;
            if ((activeConnection != null) && activeConnection.IsConnected)
            {
                return true;
            }
            return false;
        }
    }
}