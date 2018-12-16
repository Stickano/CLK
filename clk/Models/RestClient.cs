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
        public string get(string queryString)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage result = client.GetAsync(url + queryString).Result;
                if (result.IsSuccessStatusCode)
                {
                    HttpContent response = result.Content;
                    return response.ReadAsStringAsync().Result;
                }
            }
            return"";
        }

        /// <summary>
        /// Make a POST request to the RESTful interface.
        /// </summary>
        /// <typeparam name="T">A generic object - Will be Json serialized.</typeparam>
        /// <param name="obj">Object (will be formatted to Json) to put in POST request</param>
        /// <param name="queryString">The URL query string (/create/some/data   ie)</param>
        /// <returns></returns>
        public string post<T>(T obj, string queryString)
        {
            
            var serializedJson = JsonConvert.SerializeObject(obj);
            Console.WriteLine(serializedJson);
            HttpWebRequest request = WebRequest.Create(url+queryString) as HttpWebRequest;
            var enc = new UTF8Encoding(true);
            var data = enc.GetBytes(serializedJson);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var reader = request.GetRequestStream())
            {
                reader.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //WebHeaderCollection header = response.Headers;

            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                return responseText;
            }
        }
    }
}
