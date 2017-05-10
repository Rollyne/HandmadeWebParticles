
using System.IO;
using SimpleHttpServer.Enums;
using SimpleHttpServer.Models;

namespace SimpleHttpServer
{
    public static class HttpResponseBuilder
    {
        private static string resourcesPath = @"C:\Users\princ\OneDrive\Documents\visual studio 2017\Projects\Handmade Web Particles\SimpleHttpServer\Resources\Pages\";
        public static HttpResponse InternalServerError()
        {
            string content = File.ReadAllText($"{resourcesPath}500.html");

            return new HttpResponse()
            {
                StatusCode = ResponseStatusCode.InternalServerError,
                ContentAsUTF8 = content
            };
        }

        public static HttpResponse NotFound()
        {
            string content = File.ReadAllText($"{resourcesPath}404.html");

            return new HttpResponse()
            {
                StatusCode = ResponseStatusCode.NotFound,
                ContentAsUTF8 = content
            };
        }
    }
}
