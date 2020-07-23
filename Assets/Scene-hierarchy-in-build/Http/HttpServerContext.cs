using System.Net;

namespace Http
{
    public class HttpServerContext
    {
        private HttpListenerContext HttpListenerContext { get;}
        public HttpServerContext(HttpListenerContext httpListenerContext)
        {
            HttpListenerContext = httpListenerContext;
        }

        public HttpListenerResponse GetResponse()
        {
            return HttpListenerContext.Response;
        }
    }
}