using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Http
{
    public class HttpServer : IDisposable
    {
        private HttpListener _listener;
        private int _port;
        
        private Func<HttpServerContext , Task<string>> _onResponseHandler;

        public HttpServer(int port , Func<HttpServerContext , Task<string>> onResponseHandler)
        {
            _port = port;
            _onResponseHandler = onResponseHandler;
        }

        public void StartAsync()
        {
            Task.Run(WorkThreadAsync);
        }

        private void WorkThreadAsync()
        {
            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add("http://*:" + _port + "/");
                _listener.Start();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            while (_listener != null && _listener.IsListening)
            {
                HttpListenerContext context = _listener?.GetContext();
                HttpServerContext serverContext = new HttpServerContext(context);
                Task.Run(() =>
                {
                    WorkHandle(serverContext);
                });
            }
        }
        private async void WorkHandle(HttpServerContext listenerContext)
        {
            if (_onResponseHandler != null)
            {
                var returnHandler = await _onResponseHandler(listenerContext);
                
                HttpListenerResponse response = listenerContext.GetResponse();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(returnHandler);
                response.ContentLength64 = buffer.Length;
                using (System.IO.Stream output = response.OutputStream)
                {
                    await output.WriteAsync(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }

        public void Dispose()
        {
            _listener?.Stop();
            _listener?.Close();
            _listener = null;
        }
    }
}