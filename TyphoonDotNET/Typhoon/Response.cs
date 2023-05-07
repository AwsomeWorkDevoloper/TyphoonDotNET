using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Typhoon
{
    public class Response
    {
        private HttpListenerResponse _response { get; }

        private int StatusCode {
            get => StatusCode;
            set
            {
                StatusCode = value;
                _response.StatusCode = StatusCode;
            }
        }

        public Response(HttpListenerResponse res)
        {
            _response = res;
        }

        public void Send(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            _response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void SendFile(string path)
        {
            if (!File.Exists(path))
            {
                Utilities.ColorWrite($"TyphoonError <ResponseError>: file at '{path}' does not exist.", ConsoleColor.Red);

                return;
            }

            string data = File.ReadAllText(path);

            Send(data);
        }

        public void Json(object obj)
        {
            string data = JsonConvert.SerializeObject(obj);

            Send(data);
        }

        public void Redirect(string url = "/")
        {
            _response.Redirect(url);
        }
    }
}
