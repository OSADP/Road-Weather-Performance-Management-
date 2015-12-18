using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CITOMobileCommon.Models;
using System.Net.Http;

namespace CITOMobileCommon.Cloud
{
    public class CITOCloudApi: CloudCommunicationBase
    {

        public CITOCloudApi(String baseURL) :base(baseURL)
        {
        }

        public async Task<MotoristAlerts> GetMotoristAlertsForLocation(Coordinates location)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("Latitude", location.Latitude);
                parameters.Add("Longitude", location.Longitude);
                parameters.Add("Heading", location.Heading);

                return await GetFromWebService<MotoristAlerts>("/MotoristAlert", parameters);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<PostResult> UploadExtendedProbePoint(ExtendedProbe probePoint)
        {
            try
            {
                String url = "/ProbePoints";

                return await PostData(probePoint, url);
            }
            catch (HttpRequestException)
            {
                throw new TimeoutException();
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }
            catch (Exception)
            {
                throw new TimeoutException();
            }
        }

        


        public async Task<PostResult> UploadExtendedProbePoints(List<ExtendedProbe> probePoints)
        {
            try
            {
                String url = "/RWProbe";
                return await PostData(probePoints, url);
            }
            catch (HttpRequestException)
            {
                throw new TimeoutException();
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }
            catch (Exception)
            {
                throw new TimeoutException();
            }
        }
    }
}
