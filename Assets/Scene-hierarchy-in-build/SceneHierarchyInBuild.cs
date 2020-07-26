using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Http;
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
        ResponseData responseData = new ResponseData();
        
        string absolutePath = context.AbsolutePath;
        
        Debug.Log(absolutePath);

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
        }

        
        /*
        if (absolutePath.StartsWith("/hierarchy"))
        {
            await UniTask.SwitchToMainThread();
            
            _lastData = HierarchyTools.GetHierarchyActiveScene();
            finalHtml = TreeHtmlMake.InsertCodeInHtml(_lastData.rootNode, treeHtmlFile.text);
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
            
            finalHtml = sb.ToString();
        }
        
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            
            sb.AppendLine("<frameset cols=\"30%,70%\">");
            sb.AppendLine("<frame src=\"./hierarchy\" name=\"frame1\">");
            sb.AppendLine($"<frame src=\"./hierarchy\" name=\"frame2\">");
            sb.AppendLine("</frameset>");
            sb.AppendLine("</html>");
            finalHtml = sb.ToString();
        }
        */

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


    private void OnDestroy()
    {
        _httpServer.Dispose();
        _httpServer = null;
    }
}
