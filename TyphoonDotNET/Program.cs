using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Typhoon;

namespace TyphoonDotNET
{
    internal class Program
    {
        static void Main(string[] args)
        {
            App app = new App(new[] {"http://localhost:5500/"} );

            app.Use(Middleware.Log);
            app.Use(new StaticMiddleware(@"./test/public/").InvokeAsync);

            Routes routes = new Routes();
            ApiRoutes api = new ApiRoutes();

            app.Get("/", routes.Index);
            app.Get("/api/test", api.GetUser);

            app.Post("/api/test", api.PostUser);

            app.Listen();
        }
    }

    public class Routes
    {
        public async Task Index(Request req, Response res)
        {
            res.SendFile(@"./test/views/index.html");
        }
    }

    public class ApiRoutes
    {
        List<User> users = new List<User>()
        {
            new User("John", "test", "sashema09@gmail.com")
        };

        public async Task GetUser(Request req, Response res)
        {
            res.Json(users[0]); // test
        }

        public async Task PostUser(Request req, Response res)
        {
            var body = req.Body;

            Console.WriteLine(body);

            User user = JsonConvert.DeserializeObject<User>(body);

            users.Add(user);

            Console.WriteLine(JsonConvert.SerializeObject(users));

            res.Json(users);
        }
    }

    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public User(string name, string password, string email)
        {
            Name = name;
            Password = password;
            Email = email;
        }
    }
}
