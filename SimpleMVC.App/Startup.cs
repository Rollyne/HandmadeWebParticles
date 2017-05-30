using SimpleHttpServer.Models;
using SimpleMVC.App.MVC;

namespace SimpleMVC.App
{
    public class Startup
    {
        static void Main(string[] args)
        {
            HttpServer server = new HttpServer(8081, RouteTable.Routes);
            MvcEngine.Run(server);
        }
    }
}
