namespace CITOMobileCommon.VITAL.Commands
{
    public class GetTimeCommand : VitalCommand
    {
        public GetTimeCommand()
        {
            Name = "GetTime";
            Command = "%GET TIME;";
        }

        public override string Command { get; }
    }
}