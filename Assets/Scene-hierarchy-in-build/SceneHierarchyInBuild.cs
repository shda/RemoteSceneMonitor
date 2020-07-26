using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Http;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SceneHierarchyInBuild : MonoBehaviour
{
    private const string rootResourceFolder = "SceneHierarchyResources";
    
    [SerializeField]
    private TextAsset treeHtmlFile;
    private HttpServer _httpServer;

    private SceneHierarchyData _lastData;
    private ResourceFileStorage _resourceFileStorage;

    [SerializeField]
    private int port = 1234;
    [SerializeField]
    private float updateDelay = 0.2f;
    
    private void Awake()
    {
        _resourceFileStorage = new ResourceFileStorage(rootResourceFolder);
        
        _httpServer = new HttpServer(port ,OnResponseHandler);
        _httpServer.StartAsync();

#if UNITY_EDITOR
        Process.Start($"http://localhost:{port}");
#endif
    }
    
    private async Task<ResponseData> OnResponseHandler(HttpServerContext context)
    {
        ResponseData responseData = null;
        
        try
        {
            responseData = await CreateRequest(context);
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

    private async UniTask<ResponseData> CreateRequest(HttpServerContext context)
    {
        ResponseData responseData = new ResponseData {data = new byte[0]};

        string absolutePath = context.AbsolutePath;
        
        Debug.Log(absolutePath);

        if(absolutePath.StartsWith("/json/hierarhy"))
        {
            await UniTask.SwitchToMainThread();
            _lastData = HierarchyTools.GetHierarchyActiveScene();
            
            var json =  JsonConvert.SerializeObject(_lastData , Formatting.Indented);
            responseData.data = Encoding.UTF8.GetBytes(json);
        }
        else if (absolutePath.StartsWith("/action/move/"))
        {
            string parsing = absolutePath.Replace("/action/move/", "");
            string[] paramsUrl = parsing.Split('/');
            var idMove = int.Parse(paramsUrl[0]);
            var idTarget = int.Parse(paramsUrl[1]);

            await MoveChildren(idMove , idTarget);
        }
        else if (absolutePath.StartsWith("/action/delete/"))
        {
            string parsing = absolutePath.Replace("/action/delete/", "");
            var idDelete = int.Parse(parsing);
            await DeleteGameObjectById(idDelete);
            
        }
        else if (absolutePath.StartsWith("/id"))
        {
            string idStr = absolutePath.Replace("/id", "");
            StringBuilder sb = new StringBuilder();
            if (int.TryParse(idStr, out int idInt))
            {
                if (_lastData.gameobjectsDictonary.TryGetValue(idInt , out  GameObject go))
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
            string filePath = absolutePath.Remove(0, 1);
            
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
                responseData.data = Encoding.UTF8.GetBytes($"Error load file {filePath}");
            }
        }

        return responseData;
    }

    private async UniTask DeleteGameObjectById(int idDeleteObject)
    {
        await UniTask.SwitchToMainThread();
        
        var hierarchy = HierarchyTools.GetHierarchyActiveScene();
        hierarchy.gameobjectsDictonary.TryGetValue(idDeleteObject, out var deleteObject);
        if (deleteObject != null)
        {
            DestroyImmediate(deleteObject.gameObject);
        }
    }

    private async UniTask MoveChildren(int idMove, int idTarget)
    {
        await UniTask.SwitchToMainThread();
        
        var hierarchy = HierarchyTools.GetHierarchyActiveScene();

        hierarchy.gameobjectsDictonary.TryGetValue(idMove, out var moveObject);
        hierarchy.gameobjectsDictonary.TryGetValue(idTarget, out var targetObject);
        
        
        if(moveObject != null)
        {
            if (targetObject != null)
            {
                moveObject.transform.parent = targetObject.transform;
            }
            else
            {
                moveObject.transform.parent = null;
            }
        }
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


    private void OnDestroy()
    {
        _httpServer.Dispose();
        _httpServer = null;
    }
}
