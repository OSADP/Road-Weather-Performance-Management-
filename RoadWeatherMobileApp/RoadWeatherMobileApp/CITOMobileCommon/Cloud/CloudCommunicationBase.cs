using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace CITOMobileCommon.Cloud
{
    public class CloudCommunicationBase
    {
        private String baseUrl;
        private HttpClient httpClient;

        public CloudCommunicationBase(String baseURL)
        {
            baseUrl = baseURL;
            httpClient = GetHttpClient();
        }

        public class PostResult
        {
            public bool DidPost { get; set; }
        }

        public class PutResult
        {
            public bool DidPut { get; set; }
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.Timeout = TimeSpan.FromSeconds(60);
            return httpClient;
        }

        protected async Task<PostResult> PostData<T>(T objectToPost, String relativeUrl)
        {
            String url = baseUrl + relativeUrl;

            var serializedContent = JsonConvert.SerializeObject(objectToPost);

            HttpContent httpContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);

            PostResult result = new PostResult();
            if (response.IsSuccessStatusCode)
            {
                result.DidPost = true;
            }
            else
            {
                result.DidPost = false;
            }

            return result;
        }

        protected async Task<T> GetFromWebService<T>(String relativeUrl, Dictionary<string, object> parameters)
        {
            try
            {
                String url = baseUrl + relativeUrl;
                url = AddUrlParams(url, parameters);
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var returnData = JsonConvert.DeserializeObject<T>(responseString);

                    return returnData;
                }
                else
                {
                    String message = "Get error status code: " + response.StatusCode + " ; body: " + response.Content.ToString();
                    throw new Exception(message);
                }
            }
            catch (HttpRequestException)
            {
                throw new TimeoutException();
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }
        }

        private static string AddUrlParams(string baseUrl, Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                var stringBuilder = new StringBuilder(baseUrl);
                var hasFirstParam = baseUrl.Contains("?");

                foreach (var parameter in parameters)
                {
                    var format = hasFirstParam ? "&{0}={1}" : "?{0}={1}";
                    stringBuilder.AppendFormat(format, Uri.EscapeDataString(parameter.Key),
                        Uri.EscapeDataString(parameter.Value.ToString()));
                    hasFirstParam = true;
                }

                return stringBuilder.ToString();
            }
            else
                return baseUrl;
        }
    }
}
