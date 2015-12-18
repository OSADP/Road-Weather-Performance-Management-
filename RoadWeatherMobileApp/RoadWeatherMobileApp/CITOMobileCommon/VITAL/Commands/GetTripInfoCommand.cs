using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITOMobileCommon.VITAL.Commands
{
    class GetTripInfoCommand : VitalCommand
    {
        public GetTripInfoCommand()
        {
            Name = "GET TRIP INFO";
            Command = "%GET TRIPINFO;";
        }

        public override string Command { get; }
    }
    
}
