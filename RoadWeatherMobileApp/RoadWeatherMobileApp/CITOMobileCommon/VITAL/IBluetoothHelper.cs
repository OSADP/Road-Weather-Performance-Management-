using System;

namespace CITOMobileCommon.VITAL
{
    public interface IBluetoothHelper
    {
        /// <summary>
        /// Gets the paired bluetooth devices.
        /// </summary>
        /// <returns></returns>
        string[] GetPairedDevices();
        /// <summary>
        /// Initiates a the search for new bluetooth devices.
        /// Any located devices are returned via the DeviceFound event.
        /// </summary>
        void SearchForNewDevices();

        /// <summary>
        /// Connects to a bluetooth device.
        /// </summary>
        /// <param name="deviceMAC">The bluetooth device MAC address.</param>
        /// <returns></returns>
        bool ConnectToDevice(string deviceMAC);

        /// <summary>
        /// Disconnects from bluetooth device.
        /// </summary>
        /// <returns></returns>
        bool DisconnectFromDevice();

        /// <summary>
        /// Initiates the listener which will handle the incoming data from
        /// the connected bluetooth device.  Received data will be sent via
        /// the DataReceived event.
        /// </summary>
        void StartListening();

        /// <summary>
        /// Stops the listener which is monitoring the input stream from the
        /// connected device.
        /// </summary>
        void StopListening();

        /// <summary>
        /// Writes the specified data to the bluetooth device.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        bool Write(string data);

        /// <summary>
        /// Occurs when [data received].
        /// </summary>
        event EventHandler<string> DataReceived;

        /// <summary>
        /// Occurs when [device found].
        /// </summary>
        event EventHandler<string> DeviceFound;
    }
}
