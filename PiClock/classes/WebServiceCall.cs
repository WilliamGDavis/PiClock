using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class WebServiceCall
    {
        public string Uri { get; set; }
        public string Action { get; set; }
        public Dictionary<string, string> ParamDictionary { get; set; }
        private HttpClient HttpClient { get; set; }
        private HttpRequestMessage HttpRequest { get; set; }
        private HttpResponseMessage HttpResponse { get; set; }

        public WebServiceCall() { }

        public WebServiceCall(string uri = null, Dictionary<string, string> paramDictionary = null)
        {
            Uri = uri;
            ParamDictionary = paramDictionary;
        }

        //Connect to a web service and retrieve the data (JSON format) using the GET verb
        public async Task<HttpResponseMessage> GET_JsonFromWebApi()
        {
            if (null != Uri)
            {
                using (HttpClient = new HttpClient())
                {
                    HttpResponse = await HttpClient.GetAsync(new Uri(Uri));
                    HttpClient.Dispose();
                }
                return HttpResponse;
            }
            else
            { return null; }

        }

        //Connect to a web service and retrieve the data (JSON format) using the POST verb
        public async Task<HttpResponseMessage> POST_JsonToWebApi()
        {
            if (null != Uri && null != ParamDictionary)
            {
                using (HttpClient = new HttpClient())
                {
                    using (HttpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(Uri)))
                    {
                        HttpRequest.Content = new FormUrlEncodedContent(ParamDictionary);
                        HttpResponse = await HttpClient.SendAsync(HttpRequest);
                    }
                    HttpRequest.Dispose();
                    HttpClient.Dispose();
                }
                return HttpResponse;
            }
            else
            { return null; }
        }

        //Check for a valid connection to the Web Service using the PUT verb
        //NOT currently used in the application.  POST is being used to make updates
        public async Task<HttpResponseMessage> PUT_JsonToWebApi()
        {
            HttpResponseMessage httpResponse = null;
            if (null != Uri && null != ParamDictionary)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, new Uri(Uri)))
                    {
                        httpRequest.Content = new FormUrlEncodedContent(ParamDictionary);
                        httpResponse = await httpClient.SendAsync(httpRequest);
                    }

                    httpClient.Dispose();
                }
                return httpResponse;
            }
            else
            { return null; }
        }
    }

    class DbFunctions : WebServiceCall
    {
        //Check for a valid connection to the Web Service using the GET verb
        //Expected result: JSON - "true" or "false"
        public async Task<string> CheckDBConnection()
        {
            if (null != Uri)
            {
                using (HttpResponseMessage httpResponse = await GET_JsonFromWebApi())
                { return await httpResponse.Content.ReadAsStringAsync(); }
            }
            else
            { return null; }
        }
    }
}
