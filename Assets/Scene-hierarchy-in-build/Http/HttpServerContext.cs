using System.Net;

namespace Http
{
    public class HttpServerContext
    {
        public string AbsolutePath { get;}
        public HttpListenerContext HttpListenerContext { get;}
        public HttpServerContext(HttpListenerContext httpListenerContext)
        {
            HttpListenerContext = httpListenerContext;
            AbsolutePath = HttpListenerContext.Request.Url.AbsolutePath;
        }

        public HttpListenerResponse GetResponse()
        {
            return HttpListenerContext.Response;
        }
    }
}