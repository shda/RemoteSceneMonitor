using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
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
        private float _updateDelay;
        
        public RemoteSceneMonitor(int port, float updateDelay)
        {
            _port = port;
            _updateDelay = updateDelay;
            
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
        
            Debug.Log(context.HttpListenerContext.Request.RawUrl);

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
            else if (pathWithoutParams.StartsWith("/id"))
            {
                string idStr = pathWithoutParams.Replace("/id", "");
                StringBuilder sb = new StringBuilder();
                if (int.TryParse(idStr, out int idInt))
                {
                    if (_lastSceneHierarchyData.gameobjectsDictonary.TryGetValue(idInt , out  GameObject go))
                    {
                        sb.AppendLine("<html>");
                        {
                            await UniTask.SwitchToMainThread();
                            if (go != null)
                            {
                                CreateInformationStrings(sb, go);
                            }
                            else
                            {
                                sb.AppendLine($"<p>Gameobject is not found</p>");
                            }
                        }
                        sb.AppendLine("</html>");
                    }
                }
            
                responseData.data = Encoding.UTF8.GetBytes(sb.ToString());
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
                Debug.Log(json);
                finalArray = ResponseTools.ConvertStringToResponseData(json);
            }
            else
            {
                throw new Exception("Dont find id object " + idInt);
            }
            
            return finalArray;
        }

        private void CreateInformationStrings(StringBuilder sb, GameObject go)
        {
            string updateDelayString =
                _updateDelay.ToString("0.00", CultureInfo.InvariantCulture);

            sb.AppendLine($"<meta http-equiv=\"refresh\" content=\"{updateDelayString}\">");

            Vector3 position = go.transform.position;
            Vector3 rotation = go.transform.rotation.eulerAngles;
            Vector3 scale = go.transform.localScale;
            
            sb.AppendLine($"<p>{go.name}</p>");
            sb.AppendLine($"<p>Transform</p>");
            sb.AppendLine($"<p>Position: X = {position.x} Y = {position.y} Z = {position.z}</p>");
            sb.AppendLine($"<p>Rotation: X = {rotation.x} Y = {rotation.y} Z = {rotation.z}</p>");
            sb.AppendLine($"<p>Local scale: X = {scale.x} Y = {scale.y} Z = {scale.z}</p>");
        
            var getComponents = go.GetComponents<MonoBehaviour>();

            sb.AppendLine($"<p>Components:</p>");
        
            foreach (var component in getComponents)
            {
                var type = component.GetType();
                sb.AppendLine($"<p>{component.enabled} - {type.Name}</p>");
            }
        }


        public void Dispose()
        {
            _httpServer?.Dispose();
        }
    }
}