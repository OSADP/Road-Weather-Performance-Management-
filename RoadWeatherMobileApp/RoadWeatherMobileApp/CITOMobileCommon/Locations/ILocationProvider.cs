using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITOMobileCommon.Locations
{
    public interface ILocationProvider
    {
        event EventHandler<Models.Coordinates> LocationChanged;

        void StartMonitoring();
        void StopMonitoring();
        bool CanReachWebService();
    }
}
