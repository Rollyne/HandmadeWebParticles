
using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleHttpServer.Models
{
    public class CookieCollection : IEnumerable<Cookie>
    {
        public CookieCollection(IDictionary<string, Cookie> cookies)
        {
           this.Cookies = cookies;
        }

        public CookieCollection()
        {
            this.Cookies = new Dictionary<string, Cookie>();
        }

        public IDictionary<string, Cookie> Cookies { get; private set; }

        public IEnumerator<Cookie> GetEnumerator()
        {
            return this.Cookies.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get { return Cookies.Count; } }

        public bool Contains (string cookieKey)
        {
            return Cookies.ContainsKey(cookieKey);
        }

        public void Add(Cookie cookie)
        {
            if (Cookies.ContainsKey(cookie.Key))
            {
                Cookies[cookie.Key] = cookie;
            }
            else
            {
                Cookies.Add(cookie.Key, cookie);
            }
        }

        public Cookie this[string cookieKey]
        {
            get { return Cookies[cookieKey]; }
            set { this.Add(value); }
        }

        public override string ToString()
        { 
            return String.Join("; ", Cookies.Values);
        }
    }
}
