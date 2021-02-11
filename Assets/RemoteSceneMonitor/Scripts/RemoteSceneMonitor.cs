using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteSceneMonitor.HierarchyScene;
using RemoteSceneMonitor.Http;
using TaskLib;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RemoteSceneMonitor
{
    public class RemoteSceneMonitor : IDisposable
    {
        private HttpServer _httpServer;
        private ResponseFactory _responseFactory;
        private int _port;

        public RemoteSceneMonitor(int port, PackingInOneFile packingInOneFile)
        {
            UnityCallbackUpdate.CreateInstanceIfNeed();
            
            _port = port;
            _responseFactory = new ResponseFactory(packingInOneFile);
        
            _httpServer = new HttpServer(_port ,OnResponseHandler);
            _httpServer.StartAsync();
        }
    
        private async Task<ResponseData> OnResponseHandler(HttpServerContext context)
        {
            ResponseData responseData = null;
        
            try
            {
                responseData = await CreateResponse(context);
            }
            catch (Exception e)
            {
                responseData = new ResponseData
                {
                    data = ResponseTools.ConvertStringToResponseData(e.Message)
                };
                Debug.LogWarning(e.Message);
            }

            return responseData;
        }

        private async Task<ResponseData> CreateResponse(HttpServerContext context)
        {
            var pathWithoutParams = context.AbsolutePath;
            if(LogToConsoleConfig.IsLogToConsole)
            {
                Debug.Log(context.HttpListenerContext.Request.RawUrl);
            }
            
            _responseFactory.SetData(context);
            
            var response =  await _responseFactory.CreateResponse(pathWithoutParams);
            response.SetInfo(context);
            return await response.MakeResponseData();
        }

        public void Dispose()
        {
            _httpServer?.Dispose();
        }
    }
}