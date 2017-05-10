
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using SimpleHttpServer.Enums;
using SimpleHttpServer.Utilities;

namespace SimpleHttpServer.Models
{
    public class HttpProcessor
    {
        private IList<Route> routes;
        private HttpRequest request;
        private HttpResponse response;

        public HttpProcessor(IEnumerable<Route> routes)
        {
            this.routes = new List<Route>(routes);
        }

        public void HandleClient(TcpClient tcpClient)
        {
            using (var stream = tcpClient.GetStream())
            {
                this.request = GetRequest(stream);
                this.response = RouteRequest();
                StreamUtils.WriteResponse(stream, this.response);
            }
        }

        private HttpRequest GetRequest(NetworkStream stream)
        {
            var line = StreamUtils.ReadLine(stream);
            var args = line
                .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            var method = (RequestMethod) Enum.Parse(typeof(RequestMethod), args[0]);
            var url = args[1];
            var header = ReadHeader(stream);
            string content = null;

            if (header.ContentLength != null)
            {
                int totalBytes = Convert.ToInt32(header.ContentLength);
                int bytesLeft = totalBytes;
                byte[] bytes = new byte[totalBytes];

                while (bytesLeft > 0)
                {
                    byte[] buffer = new byte[bytesLeft > 1024 ? 1024 : bytesLeft];
                    int n = stream.Read(buffer, 0, buffer.Length);
                    buffer.CopyTo(bytes, totalBytes-bytesLeft);

                    bytesLeft -= n;
                }

                content = Encoding.ASCII.GetString(bytes);
            }
            var result = new HttpRequest()
            {
                Content = content,
                Method = method,
                Url = url,
                Header = header
            };

            Console.WriteLine("-REQUEST-----------------------------");
            Console.WriteLine(result);
            Console.WriteLine("------------------------------");


            return result;

        }

        private Header ReadHeader(NetworkStream stream)
        {
            var header = new Header(HeaderType.HttpRequest);
            string line;
            while (!string.IsNullOrEmpty(line = StreamUtils.ReadLine(stream)))
            {
                var args = line
                    .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                switch (args[0].Trim())
                {
                    case "Content-Length":
                        header.ContentLength = args[1].Trim();
                        break;
                    case "Cookies":
                        var cookies = SplitCookies(args[1].Trim());
                        foreach (var cookie in cookies)
                        {
                            header.AddCookie(cookie);
                        }
                        break;
                    default:
                        header.OtherParameters.Add(args[0].Trim(), args[1].Trim());
                        break;
                }
            }
            return header;
        }

        private CookieCollection SplitCookies(string cookies)
        {
            var rawCookiePairs = cookies.Split(new[]{';'}, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var result = new CookieCollection();
            foreach (var rawCookiePair in rawCookiePairs)
            {
                var args = rawCookiePair.Trim().Split(new[] {'='}).ToArray();
                result.Add(new Cookie()
                {
                    Key = args[0].Trim(),
                    Value = args[1].Trim()
                });
            }
            return result;
        }

        private HttpResponse RouteRequest()
        {
            var routes = this.routes
                .Where(x => Regex.Match(request.Url, x.UrlRegex).Success)
                .ToList();

            if (!routes.Any())
                return HttpResponseBuilder.NotFound();

            var route = routes.FirstOrDefault(x => x.Method == request.Method);

            if (route == null)
                return new HttpResponse()
                {
                    StatusCode = ResponseStatusCode.MethodNotAllowed
                };

            #region FIleSystemHandler
            // extract the path if there is one
            //var match = Regex.Match(request.Url, route.UrlRegex);
            //if (match.Groups.Count > 1)
            //{
            //    request.Path = match.Groups[1].Value;
            //}
            //else
            //{
            //    request.Path = request.Url;
            //}
            #endregion


            // trigger the route handler...
            try
            {
                return route.Callable(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return HttpResponseBuilder.InternalServerError();
            }

        }

    }
}
