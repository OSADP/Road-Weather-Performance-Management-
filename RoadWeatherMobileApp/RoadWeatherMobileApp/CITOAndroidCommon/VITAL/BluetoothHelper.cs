using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Java.Util;
using CITOMobileCommon.VITAL;
using Exception = System.Exception;

[assembly: Xamarin.Forms.Dependency(typeof(CITOAndroidCommon.VITAL.BluetoothHelperAndroid))]

namespace CITOAndroidCommon.VITAL
{

    public class BluetoothHelperAndroid : BroadcastReceiver, IBluetoothHelper
    {
        private const string TAG = "BluetoothHelperAndroid";

        private BluetoothAdapter _btAdapter;
        private BluetoothSocket _btSocket;
        private BluetoothDevice _btDevice;
        private System.Threading.Thread _listenThread;
        private bool _stopListeningThread = false;
        private CancellationTokenSource _cancelListenTokenSource;

        //private Receiver _receiver;
        // Unique UUID for this application
        private static UUID MY_UUID = UUID.FromString("2DAEC86A-18CB-4630-A828-ED184EB37C08");

        private List<string> _pairedDevices = new List<string>();
        
        public event EventHandler<string> DataReceived;
        public event EventHandler<string> DeviceFound;

        public BluetoothHelperAndroid()
        {
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            Application.Context.RegisterReceiver(this, filter);

            // Register for broadcasts when discovery has finished
            filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            Application.Context.RegisterReceiver(this, filter);

            UpdatePairedDevices();    
        }

        string[] IBluetoothHelper.GetPairedDevices()
        {
            UpdatePairedDevices();
            return _pairedDevices.ToArray();
        }


        void IBluetoothHelper.SearchForNewDevices()
        {
            // If we're already discovering, stop it
            if (_btAdapter.IsDiscovering)
            {
                _btAdapter.CancelDiscovery();
            }

            // Request discover from BluetoothAdapter
            _btAdapter.StartDiscovery();
        }

        public bool ConnectToDevice(string deviceMAC)
        {

            try
            {
                _btAdapter = BluetoothAdapter.DefaultAdapter;
                _btDevice = _btAdapter.GetRemoteDevice(deviceMAC);                
                _btSocket = _btDevice.CreateRfcommSocketToServiceRecord(MY_UUID);

                _btSocket.Connect();
            }
            catch (Java.IO.IOException)
            {
                //Failed opening BT Socket the 'easy' way.  Let's try a harder approach.
                try
                {
                    IntPtr createRfcommSocket = JNIEnv.GetMethodID(_btDevice.Class.Handle, "createRfcommSocket", "(I)Landroid/bluetooth/BluetoothSocket;");
                    IntPtr socket = JNIEnv.CallObjectMethod(_btDevice.Handle, createRfcommSocket, new JValue(1));
                    _btSocket = Java.Lang.Object.GetObject<BluetoothSocket>(socket, JniHandleOwnership.TransferLocalRef);                   

                    _btSocket.Connect();
                }
                catch (Java.IO.IOException e2)
                {
                    Log.Error(TAG, "unable to open bt socket", e2);
                    return false;
                }                
            }

            return true;
        }

        public bool DisconnectFromDevice()
        {
            try
            {
                _btSocket.Close();
            }
            catch (Java.IO.IOException e)
            {
                Log.Error(TAG, "close() of connect socket failed", e);
            }
            
            Log.Info(TAG, "Closing BT Socket. Socket IsConnected = "+ _btSocket.IsConnected);
            return !_btSocket.IsConnected;
        }

        public bool Write(string data)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(data + "\r");
            _btSocket.OutputStream.Write(byteArray, 0, byteArray.Length);
            _btSocket.OutputStream.Flush();
            return true;
        }

        public void StartListening()
        {
            _cancelListenTokenSource = new CancellationTokenSource();
            _stopListeningThread = false;
            //Java.Lang.Thread tempThread = new Java.Lang.Thread();
            _listenThread = new System.Threading.Thread(ListenToConnection)
            {
                Name = "ListenThread"
            };

            _listenThread.Start();

        }

        public void StopListening()
        {
            _stopListeningThread = true;
            _cancelListenTokenSource?.Cancel();

            if (_listenThread != null)
            {
                _listenThread.Interrupt();
                if (!_listenThread.Join(2000))
                {
                    _listenThread.Abort();
                }
            }
        }

        private async void ListenToConnection()
        {
            try
            {
                byte[] buffer = new byte[1024];
                using (Stream inStream = _btSocket.InputStream)
                {
                    while (!_stopListeningThread)
                    {
                        Array.Clear(buffer, 0, buffer.Length);

                        // Read from the InputStream
                        int bytes = await inStream.ReadAsync(buffer, 0, buffer.Length, _cancelListenTokenSource.Token);

                        Log.Info(TAG, "Num Bytes: " + bytes);

                        if (bytes > 0)
                        {
                           
                            string msg = Encoding.ASCII.GetString(buffer, 0, bytes);

                            if (DataReceived != null)
                                DataReceived(this, msg);
                            Log.Info(TAG, msg);

                        }
                    }
                }
            }
            catch (Java.IO.IOException ex3)
            {
                //We get this exception when the BT Socket is closed before the ReadAsync is canceled.  For some reason,
                //this is happening each time.  It appears that the CancelToken is not being used to cancel the ReadAsync call
                Log.Error(TAG, "Java.IO.IOException Exception in ListenToConnection", ex3);
            }
            catch (Exception e1)
            {
                Log.Error(TAG, "Generic Exception in ListenToConnection", e1);
            }
            //catch (ThreadInterruptedException e)
            //{
            //    Log.Error(TAG, "Thread Interupt in ListenToConnection", e);
            //}
            //catch (ThreadAbortException e2)
            //{
            //    Log.Error(TAG, "Thread Abort in ListenToConnection", e2);
            //}
        }

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            // When discovery finds a device
            if (action == BluetoothDevice.ActionFound)
            {
                // Get the BluetoothDevice object from the Intent
                BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                // If it's already paired, skip it, because it's been listed already
                if (device.BondState != Bond.Bonded)
                {
                    string deviceName = device.Name + "\n" + device.Address;
                    if (DeviceFound != null)
                        DeviceFound(this, deviceName);
                }
                // When discovery is finished, change the Activity title
            }
            else if (action == BluetoothAdapter.ActionDiscoveryFinished)
            {
                //_chat.SetProgressBarIndeterminateVisibility(false);
                //_chat.SetTitle(Resource.String.select_device);
                //if (newDevicesArrayAdapter.Count == 0)
                //{
                //    var noDevices = _chat.Resources.GetText(Resource.String.none_found).ToString();
                //    newDevicesArrayAdapter.Add(noDevices);
                //}
            }
        }
        private void UpdatePairedDevices()
        {
            _pairedDevices.Clear();
			// Get the local Bluetooth adapter
            _btAdapter = BluetoothAdapter.DefaultAdapter;
            var pairedDevices = _btAdapter.BondedDevices;
			// If there are paired devices, add each one to the ArrayAdapter
            if (pairedDevices.Count > 0)
            {
                foreach (var device in pairedDevices)
                {
                    _pairedDevices.Add(device.Name + "\n" + device.Address);
                }
            }
        }
    }
}