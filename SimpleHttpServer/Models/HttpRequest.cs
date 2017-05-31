
using System;
using System.Text;
using SimpleHttpServer.Enums;

namespace SimpleHttpServer.Models
{
    public class HttpRequest
    {
        public HttpRequest(Header header)
        {
            this.Header = header;
        }

        public HttpRequest()
            :this(new Header(HeaderType.HttpRequest))
        {
            
        }
        public RequestMethod Method { get; set; }
        
        public string Url { get; set; }

        public Header Header { get; set; }

        public string Content { get; set; }

        public HttpSession Session { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{Method} {Url} HTTP/1.0");
            sb.AppendLine(Header.ToString());
            if (!string.IsNullOrEmpty(Content))
            {
                sb.AppendLine(Content);
            }

            return sb.ToString();
        }
    }
}
