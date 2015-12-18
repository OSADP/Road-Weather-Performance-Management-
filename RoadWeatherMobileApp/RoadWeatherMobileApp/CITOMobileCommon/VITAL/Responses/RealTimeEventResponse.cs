namespace CITOMobileCommon.VITAL.Responses
{
    public class RealTimeEventResponse : VitalResponse
    {
        public RealTimeEventResponse(string response) : base(response)
        {
            Name = "RealTimeEventResponse";
            ResponseType = VitalResponseType.RealTimeEvent;

            string[] responseParts = response.Split(';');

            int intCode;
            if (int.TryParse(responseParts[1], out intCode))
            {
                Code = intCode;
            }

            EventName = LookupEventName(intCode);
        }

        private string LookupEventName(int intCode)
        {
            switch (intCode)
            {
                case 160:
                    return "Vehicle Identification";
                case 161:
                    return "Trip Start";
                case 162:
                    return "Trip End";
                case 163:
                    return "High Speed Event";
                case 164:
                    return "High Accel Event";
                case 165:
                    return "High Decel Event";
                case 170:
                    return "High Speed Event";
                case 176:
                    return "High Accel Event Start";
                case 177:
                    return "High Devel Event Start";
                case 178:
                    return "High Speed Event Distance";
                default:
                    return "Unknown RTE Code";
            }
        }

        public int Code { get; set; }
        public string EventName { get; set; }
    }
}