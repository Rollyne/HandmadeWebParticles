using System;
using SimpleHttpServer.Models;

namespace SimpleHttpServer.Utilities
{
    public static class SessionCreator
    {
        public static HttpSession Create()
        {
            var sessionId = new Random().Next().ToString();
            return new HttpSession(sessionId);
        }
    }
}
