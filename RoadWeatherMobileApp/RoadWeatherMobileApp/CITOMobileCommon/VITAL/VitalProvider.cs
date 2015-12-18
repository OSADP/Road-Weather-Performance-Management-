using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CITOMobileCommon.VITAL
{
    public class VitalProvider
    {
        public event EventHandler<VitalDevice> DeviceFound;
        public event EventHandler DeviceSearchComplete;

        public static List<VitalDevice> ReturnPairedDevices()
        {
            List<VitalDevice> deviceList = new List<VitalDevice>();

            IBluetoothHelper btHelper = DependencyService.Get<IBluetoothHelper>();

            string[] pairedDevices = btHelper.GetPairedDevices();
            foreach (string pairedDevice in pairedDevices)
            {
                VitalDevice device = new VitalDevice
                {
                    MacAddress = pairedDevice.Substring(pairedDevice.Length - 17),
                    DeviceName = pairedDevice.Substring(0, pairedDevice.Length - 18)
                };
                deviceList.Add(device);
            }

            return deviceList;

        }
        public void SearchForUnpairedDevices()
        {
            IBluetoothHelper btHelper = DependencyService.Get<IBluetoothHelper>();
            btHelper.DeviceFound += BtHelper_DeviceFound;

            btHelper.SearchForNewDevices();

        }

        private void BtHelper_DeviceFound(object sender, string e)
        {
            VitalDevice device = new VitalDevice
            {
                MacAddress = e.Substring(e.Length - 17),
                DeviceName = e.Substring(0, e.Length - 18)
            };

            OnDeviceFound(device);
        }

        protected virtual void OnDeviceFound(VitalDevice e)
        {
            DeviceFound?.Invoke(this, e);
        }

        protected virtual void OnDeviceSearchComplete()
        {
            DeviceSearchComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}
