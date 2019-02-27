using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace clk.Models
{
    class RestClient
    {
        private string url;

        /// <summary>
        /// Constructor.
        /// Will take the URL that you wanna make requests to,
        /// our REST interface - Or while building and testing, localhost.
        /// </summary>
        /// <param name="url"></param>
        public RestClient(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Make a GET request to the REST interface.
        /// Will return whatever result from the server in a string.
        /// </summary>
        /// <param name="queryString">The URL query</param>
        /// <returns>String of content from the server</returns>
        public string get(string queryString = "")
        {
            string queryUrl = url;
            if (!queryString.Equals(""))
                queryUrl += queryString;
            
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage result = client.GetAsync(queryUrl).Result;
                if (result.IsSuccessStatusCode)
                {
                    HttpContent response = result.Content;
                    return response.ReadAsStringAsync().Result;
                }
            }
            return"";
        }

        /// <summary>
        /// Make POST and PUT request to the REST service.
        /// The Object to POST/PUT ill be Json serialized.
        /// </summary>
        /// <param name="obj">The object to POST/PUT for the server</param>
        /// <param name="urlQuery">The servers URL query (i.e. /user/login)</param>
        /// <param name="put">If TRUE a PUT request will be made, instead of POST</param>
        /// <typeparam name="T">A generic object</typeparam>
        /// <returns>The response from the REST interface</returns>
        public string post<T>(T obj, string urlQuery, bool put = false)
        {
            
            var serializedJson = JsonConvert.SerializeObject(obj);
            var request = WebRequest.Create(url + urlQuery) as HttpWebRequest;
            var utf8 = new UTF8Encoding(true);
            var data = utf8.GetBytes(serializedJson);
            var encoding = ASCIIEncoding.ASCII;

            string method = "POST";
            if (put)
                method = "PUT";
            
            request.Method = method;
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var reader = request.GetRequestStream())
            {
                reader.Write(data, 0, data.Length);
            }
            
            //WebHeaderCollection header = response.Headers;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(
                                    response.GetResponseStream(), encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
