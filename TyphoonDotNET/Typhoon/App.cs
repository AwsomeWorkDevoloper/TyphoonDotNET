using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Typhoon
{
    internal class App
    {
        private readonly List<Func<HttpListenerContext, Task>> _middleware;
        private readonly HttpListener _listener;
        private readonly List<Handler> _handlers;

        public App(string[] prefixes)
        {
            _middleware = new List<Func<HttpListenerContext, Task>>();
            _listener = new HttpListener();
            _handlers = new List<Handler>();

            foreach (string prefix in prefixes)
            {
                _listener.Prefixes.Add(prefix);

                Utilities.ColorWrite("App running at: ", ConsoleColor.Green, "");
                Console.WriteLine(prefix);
            }
        }

        public void Use(Func<HttpListenerContext, Func<Task>, Task> middleware)
        {
            _middleware.Add(async (context) =>
            {
                await middleware(context, () => Task.FromResult(0));
            });
        }

        public void Use(Func<HttpListenerContext, Task> middleware)
        {
            _middleware.Add(middleware);
        }

        public void Listen()
        {
            try
            {
                _listener.Start();

                Console.WriteLine("Succesfully listening for requests.\n---------------------------------------");

                while (true)
                {
                    HttpListenerContext context = _listener.GetContext();
                    HandleRequest(context);
                }
            }
            catch(HttpListenerException err)
            {
                Utilities.LogError(err);
            }
        }

        private async void HandleRequest(HttpListenerContext context)
        {
            foreach (var middleware in _middleware)
            {
                await middleware(context);
            }

            var rawUrl = context.Request.RawUrl;
            var method = context.Request.HttpMethod;

            Handler handler = _handlers.Find((x) => x.Route == rawUrl && x.Method == method);

            if (handler != null)
            {
                await handler.Response(new Request(context.Request), new Response(context.Response));
                context.Response.OutputStream.Close();
            }
        }

        public void Get(string route, Func<Request, Response, Task> response)
        {
            _handlers.Add( new Handler(route, "GET", response) );
        }

        public void Post(string route, Func<Request, Response, Task> response)
        {
            _handlers.Add(new Handler(route, "POST", response));
        }
    }

    static class Utilities
    {
        public static void ColorWrite(string message, ConsoleColor color, string end="\n")
        {
            var ogColor = Console.ForegroundColor;

            Console.ForegroundColor = color;

            Console.Write(message + end);

            Console.ForegroundColor = ogColor;
        }

        public static void LogError(Exception err)
        {
            ColorWrite($"Typhoon Error: {err.Message}", ConsoleColor.Red);
        }
    }
}
