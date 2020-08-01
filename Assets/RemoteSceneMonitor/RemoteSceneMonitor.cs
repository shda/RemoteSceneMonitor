using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Http;
using Newtonsoft.Json;
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
    
        [SerializeField]
        private int port = 1234;
        [SerializeField]
        private float updateDelay = 0.2f;
    
        public RemoteSceneMonitor()
        {
            _resourceFileStorage = new ResourceFileStorage(rootResourceFolder);
            _gameObjectActionHandler = new GameObjectActionHandler();
        
            _httpServer = new HttpServer(port ,OnResponseHandler);
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
                Console.WriteLine(e);
            
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(e.Message);
                sb.AppendLine(e.StackTrace);

                Debug.LogException(e);
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

        private void CreateInformationStrings(StringBuilder sb, GameObject go)
        {
            string updateDelayString =
                updateDelay.ToString("0.00", CultureInfo.InvariantCulture);

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