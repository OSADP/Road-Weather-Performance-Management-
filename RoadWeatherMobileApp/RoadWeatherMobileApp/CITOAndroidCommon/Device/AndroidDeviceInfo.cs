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
using Android.Telephony;
using CITOMobileCommon.Device;
[assembly: Xamarin.Forms.Dependency(typeof(CITOAndroidCommon.Device.AndroidDeviceInfo))]

namespace CITOAndroidCommon.Device
{
    class AndroidDeviceInfo : IDeviceInfo
    {
        public string AppVersion()
        {
            return Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0).VersionName;
        }

        public string DeviceId()
        {
            TelephonyManager tm = (TelephonyManager)Application.Context.GetSystemService(Context.TelephonyService);

            if(tm!=null)
            {
                if (!string.IsNullOrEmpty(tm.DeviceId))
                    return tm.DeviceId;
            }

            return "unk";
        }
    }
}