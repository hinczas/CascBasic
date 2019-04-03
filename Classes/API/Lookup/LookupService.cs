using CascBasic.Models.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CascBasic.Classes
{
    public class LookupService
    {
        private string _baseUrl = "https://lookup-test.csx.cam.ac.uk/";
                
        public IbisResult InvokeMethod(string path,
                                   string[] pathParams,
                                   object[] queryParams)
        {
            return InvokeMethod(path, pathParams, queryParams, null);
        }

        public IbisResult InvokeMethod(string path, string[] pathParams, object[] queryParams, object[] formParams)
        {   
            Uri url = BuildURL(path, pathParams, queryParams);
            HttpClient client = new HttpClient();
            client.BaseAddress = url;

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();
                var model = JsonConvert.DeserializeObject<LookupResult>(jsonString.Result);
                client.Dispose();
                return model.result;
            }
            else
            {
                client.Dispose();
                return null;
            }
        }


    protected Uri BuildURL(string path, string[] pathParams, object[] queryParams)
        {
            StringBuilder sb = new StringBuilder(_baseUrl);
            bool haveQueryParams = false;
            bool haveFlattenParam = false;

            // Substitute any path parameters
            path = string.IsNullOrEmpty(path) ? "" : path;
            if (pathParams != null)
            {
                object[] encodedPathParams = new object[pathParams.Length];
                for (int i=0; i<pathParams.Length; i++)
                    if (pathParams[i] != null)
                        encodedPathParams[i] = HttpUtility.UrlEncode(pathParams[i], System.Text.Encoding.UTF8);
                path = string.Format(path, encodedPathParams);
            }

            // Add the path to the common URL base
            if (sb[sb.Length-1] != '/')
                sb.Append('/');

            while (path.StartsWith("/"))
                path = path.Substring(1);

            while (path.EndsWith("/"))
                path = path.Substring(0, path.Length-1);

            if (!string.IsNullOrEmpty(path))
                sb.Append(path);

            // Add any query parameters
            if (queryParams != null)
            {
                for (int i=0; i<queryParams.Length-1; i+=2)
                {
                    if (queryParams[i] != null && queryParams[i + 1] != null)
                    {
                        string name = queryParams[i].ToString();
                        string value = queryParams[i + 1].GetType()==typeof(DateTime) ?
                                        ((DateTime)queryParams[i+1]).ToShortDateString() :
                                           queryParams[i + 1].GetType() == typeof(IbisAttribute)  ?
                                           ((IbisAttribute ) queryParams[i + 1]).EncodedString() :
                                           queryParams[i + 1].ToString();

                    sb.Append(haveQueryParams? '&' : '?');
                    sb.Append(HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8));
                    sb.Append('=');
                    sb.Append(HttpUtility.UrlEncode(value, System.Text.Encoding.UTF8));
                    haveQueryParams = true;
                    if (name.Equals("flatten"))
                        haveFlattenParam = true;
                    }
                }
            }

            // If the flattened XML representation is being used, add the
            // "flatten" parameter, unless it has already been specified
            if (!haveFlattenParam)
            {
                sb.Append(haveQueryParams ? '&' : '?');
                sb.Append("flatten=false");
            }

            // Finally create and return the full URL
            Uri url = new Uri(sb.ToString());
            if (!"https".Equals(url.Scheme, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Illegal URL protocol - must use HTTPS");

            return url;
        }
    }
}