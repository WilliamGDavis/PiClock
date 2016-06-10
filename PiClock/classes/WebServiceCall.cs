using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class WebServiceCall
    {
        //URI of the web service resource
        public string Uri { get; set; }
        //Dictionary of paramaters to pass to the web service
        public Dictionary<string, string> ParamDictionary { get; set; }

        //Optionally pass in the uri and paramDictionary
        public WebServiceCall(Dictionary<string, string> paramDictionary = null)
        {
            Uri = Settings.Read("UriPrefix");
            ParamDictionary = paramDictionary;
        }

        //Connect to a web service and retrieve the data (JSON format) using the POST verb
        public async Task<HttpResponseMessage> PostJsonToRpcServer()
        {
            //Return a null if the URI or the ParamDictionary is empty(null)
            if (null == Uri || null == ParamDictionary)
            { return null; }

            using (var HttpClient = new HttpClient())
            {
                //Client headers
                string basicAuth = string.Format("{0}:{1}", Settings.Read("ApiUsername"), Settings.Read("ApiPassword"));
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(basicAuth)));

                //Content headers
                var content = new StringContent(JsonConvert.SerializeObject(ParamDictionary));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                //Return JSON from the RPC server
                return await HttpClient.PostAsync(Uri, content);
            }
        }

        //Connect to a web service and retrieve the data (JSON format) using the GET verb
        public async Task<HttpResponseMessage> GET_JsonFromWebApi()
        {
            if (null == Uri || null == ParamDictionary)
            { return null; }

            //TODO: Retrieve params passed in and build the URI

            using (var HttpClient = new HttpClient())
            { return await HttpClient.GetAsync(new Uri(Uri)); }
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
        public DbFunctions(string uri, Dictionary<string, string> paramDictionary)
        {
            Uri = uri;
            ParamDictionary = paramDictionary;
        }
        public async Task<HttpResponseMessage> CheckDBConnection()
        {
            //Return a null if the URI or the ParamDictionary is empty(null)
            if (null == Uri || null == ParamDictionary)
            { return null; }

            using (var HttpClient = new HttpClient())
            {
                //Client headers
                string basicAuth = string.Format("{0}:{1}", ParamDictionary["ApiUsername"], ParamDictionary["ApiPassword"]);
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(basicAuth)));

                //Content headers
                var content = new StringContent(JsonConvert.SerializeObject(ParamDictionary));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                //return JSON from the RPC server
                return await HttpClient.PostAsync(Uri, content);
            }
        }
    }
}
