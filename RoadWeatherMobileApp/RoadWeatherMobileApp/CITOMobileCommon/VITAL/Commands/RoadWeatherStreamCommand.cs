namespace CITOMobileCommon.VITAL.Commands
{
    public class RoadWeatherStreamCommand : VitalCommand
    {
        string baseCommand = "%SET RWS";

        public RoadWeatherStreamCommand() : this(false)
        {}

        public RoadWeatherStreamCommand(bool streamOn)
        {
            Name = "RoadWeatherStream";
            StreamOn = streamOn;
        }

        public bool StreamOn { get; set; }

        public override string Command
        {
            get { return baseCommand + " " + (StreamOn == true ? "ON" : "OFF"); }            
        }
    }
}