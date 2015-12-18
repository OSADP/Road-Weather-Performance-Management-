using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITOMobileCommon.VITAL.Commands;
using CITOMobileCommon.VITAL.Responses;
using Xamarin.Forms;

namespace CITOMobileCommon.VITAL
{
    public class VitalDevice
    {
        public string MacAddress { get; set; }
        public string DeviceName { get; set; }

        public event EventHandler<VitalResponse> MessageReceived;
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        private string _responseBuffer = "";
        private readonly string endMsgDelimiter = "ENDMSG";
        private bool _isConnected;

        public bool Connect()
        {
            IBluetoothHelper btHelper = DependencyService.Get<IBluetoothHelper>();
            try {
                btHelper.DataReceived += BtHelper_DataReceived;
                bool connected = btHelper.ConnectToDevice(MacAddress);
                if (!connected) return false;
            	_isConnected = true;
                btHelper.StartListening();
                OnConnected();
                return true;
            }catch(Exception ex)
            {
                Debug.WriteLine("EXCEPTION: " + ex.Message);
            }
            return false;
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }
		
        public bool Disconnect()
        {
            IBluetoothHelper btHelper = DependencyService.Get<IBluetoothHelper>();
            btHelper.StopListening();
            btHelper.DataReceived -= BtHelper_DataReceived;

            bool disconnected = btHelper.DisconnectFromDevice();
            if (!disconnected) return false;
            _isConnected = false;
            OnDisconnected();
            return true;
        }

        public bool SendCmd(VitalCommand cmd)
        {
            IBluetoothHelper btHelper = DependencyService.Get<IBluetoothHelper>();
            return btHelper.Write(cmd.Command);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VitalDevice) obj);
        }
        protected bool Equals(VitalDevice other)
        {
            return string.Equals(MacAddress, other.MacAddress) && string.Equals(DeviceName, other.DeviceName);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((MacAddress != null ? MacAddress.GetHashCode() : 0)*397) ^ (DeviceName != null ? DeviceName.GetHashCode() : 0);
            }
        }
        public static bool operator ==(VitalDevice left, VitalDevice right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(VitalDevice left, VitalDevice right)
        {
            return !Equals(left, right);
        }

        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMessageReceived(VitalResponse e)
        {
            //MessageReceived?.Invoke(this, e);
            if (MessageReceived != null)
                MessageReceived(this, e);
        }


        private void BtHelper_DataReceived(object sender, string receivedData)
        {
            //Look for complete messages.  Once found, package them and send them up to the client.
            receivedData = receivedData.Replace("\r", "");
            receivedData = receivedData.Replace("\n", "");

            _responseBuffer = _responseBuffer + receivedData;


            int endMsgPos = _responseBuffer.IndexOf(endMsgDelimiter, StringComparison.Ordinal);
            while (endMsgPos > 0)
            {
                string rawMsg = _responseBuffer.Substring(0, endMsgPos);

                VitalResponse resp = ParseResponse(rawMsg);

                OnMessageReceived(resp);

                _responseBuffer = _responseBuffer.Substring(endMsgPos + endMsgDelimiter.Length);

                endMsgPos = _responseBuffer.IndexOf(endMsgDelimiter, StringComparison.Ordinal);
            }            
        }

        private VitalResponse ParseResponse(string rawMsg)
        {
            string[] responseParts = rawMsg.Split(';');

            if (responseParts[0].Equals("GET TIME"))
            {
                BasicStringResponse bsr = new BasicStringResponse(responseParts[1])
                {
                    ResponseType = VitalResponseType.DateTimeResponse,
                    Name = "Get Time Response"
                };

                return bsr;
            }

            if( responseParts[0].Equals("GET TRIPINFO"))
            {
                TripInfoResponse tir = new TripInfoResponse(rawMsg)
                {
                    ResponseType = VitalResponseType.TripInfoResponse,
                    Name = "Trip Info Response"
                };

                return tir;
            }

            if (responseParts[0].Equals("RTE"))
            {
                RealTimeEventResponse rte = new RealTimeEventResponse(rawMsg)
                {
                    ResponseType = VitalResponseType.RealTimeEvent,
                    Name = "Real Time Event"
                };

                return rte;
            }

            if (responseParts[0].Equals("SET RWS"))
            {
                CommandAckResponse ackResponse = new CommandAckResponse(responseParts[0], responseParts[1])
                {
                    ResponseType = VitalResponseType.CommandAckResponse,
                    Name = "SET RWS Command Ack"
                };

                return ackResponse;
            }
            if (responseParts[0].Equals("RW"))
            {
                RoadWeatherStreamResponse bsr = new RoadWeatherStreamResponse(rawMsg)
                {
                    ResponseType = VitalResponseType.RoadWeatherStream,
                    Name = "Road Weather Stream"
                };

                return bsr;
            }

            if (responseParts[0].Equals("SET DOUT"))
            {
                CommandAckResponse ackResponse = new CommandAckResponse(responseParts[0], responseParts[1])
                {
                    ResponseType = VitalResponseType.CommandAckResponse,
                    Name = "SET DOUT Command Ack"
                };
                return ackResponse;
            }
            VitalResponse unkResponse = new BasicStringResponse(rawMsg)
            {
                ResponseType = VitalResponseType.UnknkownResponse,
                Name = "Unknown Response"
            };
            return unkResponse;
        }
    }
}
