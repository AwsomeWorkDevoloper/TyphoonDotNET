using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon
{
    internal class Handler
    {
        public Handler(string route, string method, Func<Request, Response, Task> response)
        {
            Route = route;
            Method = method;
            Response = response;
        }

        public string Route { get; }
        public string Method { get; }
        public Func<Request, Response, Task> Response { get; }
    }
}
