namespace CITOMobileCommon.VITAL.Commands
{
    public abstract class VitalCommand
    {
        public string Name { get; set; }
        public abstract string Command { get; }       
    }
}
