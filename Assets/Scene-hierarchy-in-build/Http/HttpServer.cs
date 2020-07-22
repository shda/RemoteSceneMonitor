using System;
using System.Net;
using System.Threading;
using CinemaLib.Utils;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Http
{
    public class HttpServer : IDisposable
    {
        private HttpListener listener;
        private readonly int port;

        public HttpServer(int port)
        {
            this.port = port;
        }

        public void Start()
        {
            ThreadsController.StartThread(WorkThread);
        }

        private void WorkThread(object obj)
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add("http://*:" + port + "/");
                listener.Start();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            while (listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    Process(context);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    
        private void Process(HttpListenerContext context)
        {
            //Task.Run()
            
            ThreadsController.StartThread(UploadStream, context);
        }

        private void UploadStream(object obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj;
            string filename = context.Request.Url.AbsolutePath;
            Debug.Log(filename);

            HttpListenerResponse response = context.Response;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(
                "<HTML><BODY> " + "Is Run!" + "</BODY></HTML>");
       
            response.ContentLength64 = buffer.Length;
            using (System.IO.Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        public void Dispose()
        {
            listener?.Close();
        }
    }
}