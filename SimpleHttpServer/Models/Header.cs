
using System.Collections.Generic;
using System.Text;
using SimpleHttpServer.Enums;

namespace SimpleHttpServer.Models
{
    public class Header
    {
        public Header(HeaderType type,
            CookieCollection cookies,
            Dictionary<string, string> otherParameters,
            string contentType = "text/html")
        {
            this.Type = type;
            this.ContentType = contentType;
            this.Cookies = cookies;
            this.OtherParameters = otherParameters;
        }

        public Header(HeaderType type)
            :this(type, new CookieCollection(), new Dictionary<string, string>())
        {
            
        }

        public HeaderType Type { get; set; }

        public string ContentType { get; set; }

        public string ContentLength { get; set; }

        public Dictionary<string,string> OtherParameters { get; set; }

        public CookieCollection Cookies { get; private set; }

        public void AddCookie(Cookie cookie)
        {
            this.Cookies.Add(cookie);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Content-type: {this.ContentType}");
            if (this.Cookies.Count > 0)
            {
                if(this.Type == HeaderType.HttpRequest)
                    sb.AppendLine($"Cookie: {this.Cookies.ToString()}");
                else if (this.Type == HeaderType.HttpResponse)
                {
                    foreach (var cookie in this.Cookies)
                    {
                        sb.AppendLine($"Set-Cookie: {cookie}");
                    }
                }
            }
            if (this.ContentLength != null)
            {
                sb.AppendLine($"Content-Length: {this.ContentLength}");
            }
            foreach (var otherParameter in OtherParameters)
            {
                sb.AppendLine($"{otherParameter.Key}: {otherParameter.Value}");
            }
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
