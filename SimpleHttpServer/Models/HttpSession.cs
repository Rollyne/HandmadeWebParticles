using System.Collections;
using System.Collections.Generic;

namespace SimpleHttpServer.Models
{
    public class HttpSession
    {
        private IDictionary<string, string> parameters;

        public HttpSession(string id)
        {
            parameters = new Dictionary<string, string>();
            this.Id = id;
        }

        public string Id { get; set; }

        public string this[string key] => parameters["key"];

        public void Clear()
        {
            parameters = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            if (parameters.ContainsKey(key))
            {
                parameters[key] = value;
                return;
            }
            parameters.Add(key, value);
        }
    }
}
