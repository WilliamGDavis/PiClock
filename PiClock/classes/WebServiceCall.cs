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

        public WebServiceCall()
        {
            Uri = null;
            Action = null;
            ParamDictionary = null;
        }

        //Connect to a web service and retrieve the data (JSON format) using the GET verb
        public async Task<HttpResponseMessage> GET_JsonFromWebApi()
        {
            HttpResponseMessage httpResponse = null;
            if (null != Uri)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpResponse = await httpClient.GetAsync(new Uri(Uri));
                    httpClient.Dispose();
                }
                return httpResponse;
            }
            else
            { return null; }

        }

        //Connect to a web service and retrieve the data (JSON format) using the POST verb
        public async Task<HttpResponseMessage> POST_JsonToWebApi()
        {
            HttpResponseMessage httpResponse = null;
            if (null != Uri && null != ParamDictionary)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(Uri)))
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
