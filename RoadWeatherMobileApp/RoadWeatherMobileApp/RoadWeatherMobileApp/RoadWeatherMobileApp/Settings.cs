using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadWeatherMobileApp
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        private const string VitalDeviceMacKey = "vital_device";
        private static readonly string VitalDeviceMacKeyDefault = string.Empty;

        public static string VitalDeviceMac
        {
            get { return AppSettings.GetValueOrDefault<string>(VitalDeviceMacKey, VitalDeviceMacKeyDefault); }
            set { AppSettings.AddOrUpdateValue<string>(VitalDeviceMacKey, value); }
        }
    }
}
