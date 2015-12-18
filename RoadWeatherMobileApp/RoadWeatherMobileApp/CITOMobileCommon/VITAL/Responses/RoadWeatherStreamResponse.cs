namespace CITOMobileCommon.VITAL.Responses
{
    public class RoadWeatherStreamResponse : VitalResponse
    {
        public float Speed { get; set; }
        public float AmbientTemperature { get; set; }
        public float BarametricPressure { get; set; }
        public int WiperStatus { get; set; }
        public int HeadlightStatus { get; set; }
        public float FrontLeftWheelSpeed { get; set; }
        public float FrontRightWheelSpeed { get; set; }
        public float RearLeftWheelSpeed { get; set; }
        public float RearRightWheelSpeed { get; set; }

        public RoadWeatherStreamResponse(string response) : base(response)
        {
            Name = "RoadWeatherStreamResponse";
            ResponseType = VitalResponseType.RoadWeatherStream;

            string[] streamElements = response.Split(';');

            float fltSpeed;
            if (streamElements.Length >= 2 && float.TryParse(streamElements[1], out fltSpeed))
            {
                Speed = fltSpeed;
            }

            float fltAmbTemp;
            if (streamElements.Length >= 3 && float.TryParse(streamElements[2], out fltAmbTemp))
            {
                AmbientTemperature = fltAmbTemp;
            }

            float fltBarPress;
            if (streamElements.Length >= 4 && float.TryParse(streamElements[3], out fltBarPress))
            {
                BarametricPressure = fltBarPress;
            }

            int intWiperStatus;
            if (streamElements.Length >= 5 && int.TryParse(streamElements[4], out intWiperStatus))
            {
                WiperStatus = intWiperStatus;
            }

            int intHeadlightStatus;
            if (streamElements.Length >= 6 && int.TryParse(streamElements[5], out intHeadlightStatus))
            {
                HeadlightStatus = intHeadlightStatus;
            }


            float fltFrontLeft;
            if (streamElements.Length >= 7 && float.TryParse(streamElements[6], out fltFrontLeft))
            {
                FrontLeftWheelSpeed = fltFrontLeft;
            }

            float fltFrontRight;
            if (streamElements.Length >= 8 && float.TryParse(streamElements[7], out fltFrontRight))
            {
                FrontRightWheelSpeed = fltFrontRight;
            }

            float fltRearLeft;
            if (streamElements.Length >= 9 && float.TryParse(streamElements[8], out fltRearLeft))
            {
                RearLeftWheelSpeed = fltRearLeft;
            }

            float fltRearRight;
            if (streamElements.Length >= 10 && float.TryParse(streamElements[9], out fltRearRight))
            {
                RearRightWheelSpeed = fltRearRight;
            }
        }
    }
}