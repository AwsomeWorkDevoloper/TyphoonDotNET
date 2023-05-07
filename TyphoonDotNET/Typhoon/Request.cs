using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Typhoon
{
    public class Request
    {
        private HttpListenerRequest _request { get; }

        private string HttpMethod { get; }
        private string Url { get; }

        public string Body 
        {
            get 
            {
                string result = "";

                // Get the request body as a string
                using (StreamReader reader = new StreamReader(_request.InputStream))
                {
                    string requestBody = reader.ReadToEnd();
                    // Do something with the request body
                    result = requestBody;
                }

                return result;
            }
        }

        public Request(HttpListenerRequest req)
        {
            _request = req;

            HttpMethod = req.HttpMethod;
            Url = req.RawUrl;
        }
    }
}
