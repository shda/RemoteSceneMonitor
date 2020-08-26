using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RemoteSceneMonitor.HierarchyScene;
using RemoteSceneMonitor.Http;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RemoteSceneMonitor
{
    public class RemoteSceneMonitor : IDisposable
    {
        private const string rootResourceFolder = "RemoteSceneMonitorResources";
        
        private HttpServer _httpServer;
        private SceneHierarchyData _lastSceneHierarchyData;
        private ResourceFileStorage _resourceFileStorage;
        private GameObjectActionHandler _gameObjectActionHandler;
        
        private int _port;

        public RemoteSceneMonitor(int port)
        {
            _port = port;

            _resourceFileStorage = new ResourceFileStorage(rootResourceFolder);
            _gameObjectActionHandler = new GameObjectActionHandler();
        
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
            ResponseData responseData = new ResponseData {data = new byte[0]};

            var pathWithoutParams = context.AbsolutePath;
            var queryString = context.HttpListenerContext.Request.QueryString;
            
            if(LogToConsoleConfig.IsLogToConsole)
            {
                Debug.Log(context.HttpListenerContext.Request.RawUrl);
            }

            if(pathWithoutParams.StartsWith("/json/hierarchy"))
            {
                await UniTask.SwitchToMainThread();
                _lastSceneHierarchyData = HierarchyTools.GetHierarchyActiveScene();
            
                var json =  JsonConvert.SerializeObject(_lastSceneHierarchyData , Formatting.Indented);
                responseData.data = ResponseTools.ConvertStringToResponseData(json);
            }
            else if(pathWithoutParams.StartsWith("/action"))
            {
                responseData.data = await _gameObjectActionHandler.ActionRequestHandler(queryString);
            }
            else if (pathWithoutParams.StartsWith("/gameObjectInfo"))
            {
                responseData.data = await GetGameObjectInfo(queryString);
            }
            else
            {
                string filePath = pathWithoutParams.Remove(0, 1);
            
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = "index.html";
                }
            
                FileReadResult fileReadResult = await _resourceFileStorage.ReadFileFromResource(filePath);

                if (!fileReadResult.IsError)
                {
                    responseData.data = fileReadResult.data;
                }
                else
                {
                    Debug.LogError($"Error load file - {filePath}");
                    responseData.data = ResponseTools.ConvertStringToResponseData($"Error load file {filePath}");
                }
            }

            return responseData;
        }

        private async Task<byte[]> GetGameObjectInfo(NameValueCollection queryString)
        {
            var idString = queryString.Get("id");

            if (string.IsNullOrEmpty(idString))
            {
                throw new Exception("Dont find tag \"id\" in query string");
            }

            var idInt = int.Parse(idString);
            byte[] finalArray = new byte[0];
            
            if (_lastSceneHierarchyData.gameobjectsDictonary.TryGetValue(idInt, out GameObject go))
            {
                await UniTask.SwitchToMainThread();
                
                Vector3 position = go.transform.position;
                Vector3 rotation = go.transform.rotation.eulerAngles;
                Vector3 scale = go.transform.localScale;


                GameObjectInfo objectInfo = new GameObjectInfo()
                {
                    activeSelf = go.activeSelf,
                    name = go.name,
                    position = position,
                    rotation = rotation,
                    scale = scale,
                };
                
                var json =  JsonConvert.SerializeObject(objectInfo , Formatting.Indented);
                finalArray = ResponseTools.ConvertStringToResponseData(json);

                if (LogToConsoleConfig.IsLogToConsole)
                {
                    Debug.Log(json);
                }
            }
            else
            {
                throw new Exception("Dont find id object " + idInt);
            }
            
            return finalArray;
        }

        public void Dispose()
        {
            _httpServer?.Dispose();
        }
    }
}