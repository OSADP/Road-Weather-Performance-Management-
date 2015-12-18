using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITOMobileCommon.VITAL.Responses
{
    public class TripInfoResponse : VitalResponse
    {
        public TripInfoResponse(string response) : base(response)
        {
            string[] responseParts = response.Split(';');

            string isOnTripString = responseParts[2];

            int isOnTripInt = int.Parse(isOnTripString.Trim());

            if (isOnTripInt == 1)
                IsOnTrip = true;
            else
                IsOnTrip = false;
        }

        public Boolean IsOnTrip { get; private set; }
    }
}
