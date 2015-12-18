using System;
using Xamarin.Forms;

namespace CITOMobileCommon.VITAL.Responses
{
    public enum VitalResponseType
    {
        DateTimeResponse,
        MacResponse,
        RealTimeEvent,
        RoadWeatherStream,
        UnknkownResponse,
        CommandAckResponse,
        TripInfoResponse
    }

    public abstract class VitalResponse
    {
        protected VitalResponse(string response)
        {
            ResponseType = VitalResponseType.UnknkownResponse;
            ResponseString = response;
        }

        public string Name { get; set; }
        public VitalResponseType ResponseType { get; set; }

        public string ResponseString { get; set; }
    }

    public class BasicStringResponse : VitalResponse
    {
        public BasicStringResponse(string response) : base(response)
        {
            Name = "BasicStringResponse";
        }        
    }

    public class CommandAckResponse : VitalResponse
    {
        public string Command { get; set; }
        public bool Success { get; set; }
        public CommandAckResponse(string command, string response) : base(response)
        {
            Name = "Command Ack Response";
            Command = command;
            Success = response.StartsWith("0");
        }
    }
}