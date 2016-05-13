using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiClock.classes
{
    class JsonMethods
    {
        public static async Task<Employee> Json_GetEmployeeById(string Uri)
        {
            return await JsonMethods_Employee.ReturnJson_EmployeeById(Uri, "get_employee_by_id");
        }

        //public static async Task<List<Employee>> Json_GetAllEmployees(string Uri)
        //{
        //    return await JsonMethods_Employee.ReturnJson_AllEmployees(Uri, "get_all_employees");
        //}

        //public static async Task<string> CheckDBConnection(string Uri)
        //{
        //    return await JsonMethods_Database.CheckDBConnectionStatus(Uri, "test_connection");
        //}

        public static async Task<string> GetCurrentJobNumber(string Uri)
        {
            return await JsonMethods_Employee.RetrieveCurrentJob(Uri);
        }

        private class JsonMethods_Employee
        {
            public static async Task<Employee> ReturnJson_EmployeeById(string Uri, string function)
            {
                //Connect to a web service and retrieve the data (JSON format)
                var JsonString = await GET_JsonFromWebApi(Uri);

                //Determine the function to used based on the string passed in (May or may not be helpful)
                Employee model;
                switch (function)
                {
                    case "get_employee_by_id":
                        model = JsonConvert.DeserializeObject<Employee>(JsonString);
                        break;
                    default: //Return an empty string
                        model = JsonConvert.DeserializeObject<Employee>("");
                        break;
                }
                return model;
            }

            //public static async Task<List<Employee>> ReturnJson_AllEmployees(string Uri, string function)
            //{
            //    //Connect to a web service and retrieve the data (JSON format)
            //    string JsonString = await RetrieveJson(Uri);

            //    //Determine the function to used based on the string passed in (May or may not be helpful)
            //    List<Employee> model;
            //    switch (function)
            //    {
            //        case "get_all_employees":
            //            model = JsonConvert.DeserializeObject<List<Employee>>(JsonString);
            //            break;
            //        default: //Return an empty string
            //            model = null;
            //            break;
            //    }

            //    return model;
            //}

            private static async Task<string> RetrieveJson(string Uri)
            {
                //Connect to a web service and retrieve the data (JSON format)
                try
                {
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(new Uri(Uri));
                    string JsonString = await response.Content.ReadAsStringAsync();
                    response.Dispose();
                    return JsonString;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            public static async Task<string> RetrieveCurrentJob(string uri)
            {
                ////Connect to a web service and retrieve the data (JSON format)
                //try
                //{
                //    var client = new HttpClient();
                //    HttpResponseMessage response = await client.GetAsync(new Uri(Uri));
                //    string JsonString = await response.Content.ReadAsStringAsync();
                //    response.Dispose();
                //    return JsonString;
                //}
                //catch (Exception ex)
                //{
                //    return ex.Message;
                //}
                try
                {
                   return await GET_JsonFromWebApi(uri);
                } catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            //Connect to a web service and retrieve the data (JSON format)
            public static async Task<string> GET_JsonFromWebApi(string uri)
            {
                //using (var client = new HttpClient())
                //{
                //    try
                //    {
                //        HttpResponseMessage response = await client.GetAsync(new Uri(Uri));
                //        return await response.Content.ReadAsStringAsync();
                //    }
                //    catch (Exception ex)
                //    { return ex.Message; }
                //}
                try
                {
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(new Uri(uri));
                    client.Dispose();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                { return ex.Message; }
            }
        }

        //private class JsonMethods_Database
        //{
        //    public static async Task<string> CheckDBConnectionStatus(string Uri, string function)
        //    {
        //        //Connect to a web service and retrieve the data (JSON format)
        //        try
        //        {
        //            var client = new HttpClient();
        //            HttpResponseMessage response = await client.GetAsync(new Uri(Uri));
        //            string JsonString = await response.Content.ReadAsStringAsync();
        //            response.Dispose();
        //            return JsonString;
        //        }
        //        catch (Exception ex)
        //        {
        //            return ex.Message;
        //        }
        //    }
        //}
    }
}
