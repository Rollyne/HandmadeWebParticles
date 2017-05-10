
namespace SimpleHttpServer.Models
{
    public class Cookie
    {
        public Cookie(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public Cookie() :this(null, null)
        {
            
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Key}={Value}";
        }
    }
}
