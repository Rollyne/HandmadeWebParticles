
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleHttpServer.Models
{
    public class HttpServer
    {
        public HttpServer(int port, IEnumerable<Route> routes)
        {
            this.Port = port;
            this.Sessions = new Dictionary<string, HttpSession>();
            this.Processor = new HttpProcessor(routes, this.Sessions);
            this.IsActive = true;
        }
        public IDictionary<string, HttpSession> Sessions { get; set; }
        public TcpListener Listener { get; private set; }
        public int Port { get; private set; }
        public bool IsActive { get; private set; }
        public HttpProcessor Processor { get; private set; }

        public void Listen()
        {
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start();

            while (this.IsActive)
            {
                var client = Listener.AcceptTcpClient();
                var thread = new Thread(() => Processor.HandleClient(client));

                thread.Start();
                Thread.Sleep(1);
            }
        }
    }
}
