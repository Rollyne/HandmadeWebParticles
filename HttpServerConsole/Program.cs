using System.Collections.Generic;
using SimpleHttpServer.Enums;
using SimpleHttpServer.Models;

namespace HttpServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var routes = new List<Route>()
            {
                new Route()
                {
                    Name = "Hello Handler",
                    UrlRegex = @"^/hello$",
                    Method = RequestMethod.GET,
                    Callable = (HttpRequest request) =>
                    {
                        return new HttpResponse()
                        {
                            ContentAsUTF8 = "<h3>Hello from HTTPServer :) </h3>",
                            StatusCode = ResponseStatusCode.OK
                        };
                    }
                }
            };

            HttpServer server = new HttpServer(8081, routes);
            System.Console.WriteLine("Connected");
            server.Listen();

        }
    }
}
