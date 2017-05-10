
using System;
using System.Text;
using SimpleHttpServer.Enums;

namespace SimpleHttpServer.Models
{
    public class HttpResponse
    {
        public HttpResponse()
            :this(new Header(HeaderType.HttpResponse), new byte[] { })
        {
        }

        public HttpResponse(Header header, byte[] content)
        {
            Header = header;
            Content = content;
        }
        public ResponseStatusCode StatusCode { get; set; }

        public string StatusMessage => Enum.GetName(typeof(ResponseStatusCode), StatusCode);

        public Header Header { get; set; }

        public byte[] Content { get; set; }

        public string ContentAsUTF8 { set { this.Content = Encoding.UTF8.GetBytes(value); } }

        public override string ToString()
        {
            return $"HTTP/1.1 {(int)this.StatusCode} {this.StatusMessage}\r\n{this.Header}";
        }
    }
}
