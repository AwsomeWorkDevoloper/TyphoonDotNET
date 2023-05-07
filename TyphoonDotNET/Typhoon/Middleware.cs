using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace Typhoon
{
    public static class Middleware
    {
        public static async Task Log(HttpListenerContext context, Func<Task> next)
        {
            // Log the incoming request
            Console.Write($"Received ");
            Utilities.ColorWrite($"{context.Request.HttpMethod} ", ConsoleColor.Blue, "");
            Console.Write("request for ");
            Utilities.ColorWrite($"{context.Request.RawUrl}", ConsoleColor.Cyan);

            // Invoke the next middleware function in the pipeline
            await next();

            // Log the response code
            Console.Write($"Sent response with status code ");
            Utilities.ColorWrite(context.Response.StatusCode.ToString(), ConsoleColor.Blue);
        }

        public static async Task Static(HttpListenerContext context, Func<Task> next)
        {
            // Log the incoming request
            Console.Write($"Received ");
            Utilities.ColorWrite($"{context.Request.HttpMethod} ", ConsoleColor.Blue, "");
            Console.Write("request for ");
            Utilities.ColorWrite($"{context.Request.RawUrl}", ConsoleColor.Cyan);

            // Invoke the next middleware function in the pipeline
            await next();

            // Log the response code
            Console.Write($"Sent response with status code ");
            Utilities.ColorWrite(context.Response.StatusCode.ToString(), ConsoleColor.Blue);
        }
    }

    public class StaticMiddleware
    {
        private readonly string rootDirectory;
        private readonly string contentType;

        public StaticMiddleware(string rootDirectory, string contentType = "text/html")
        {
            this.rootDirectory = rootDirectory;
            this.contentType = contentType;
        }

        public async Task InvokeAsync(HttpListenerContext context, Func<Task> next)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            if (request.HttpMethod != "GET")
            {
                await next();
                return;
            }

            string path = request.Url.LocalPath.Substring(1);
            string fullPath = Path.Combine(rootDirectory, path);

            //Utilities.ColorWrite($"{path}: {fullPath}", ConsoleColor.Green);

            if (!File.Exists(fullPath))
            {
                await next();
                return;
            }

            response.ContentType = contentType;

            using (FileStream stream = File.OpenRead(fullPath))
            {
                response.ContentLength64 = stream.Length;
                await stream.CopyToAsync(response.OutputStream);
            }

            response.Close();
        }
    }

}
