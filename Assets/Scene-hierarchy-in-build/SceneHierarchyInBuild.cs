using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Assets.AR_RPG.Scripts.Utils;
using Http;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SceneHierarchyInBuild : MonoBehaviour
{
    [SerializeField]
    private TextAsset treeHtmlFile;
    private HttpServer _httpServer;

    private SceneHierarchyData lastData;

    [SerializeField]
    private int port = 1234;
    [SerializeField]
    private float updateDelay = 0.2f;
    
    private void Awake()
    {
        UnityCallbackUpdate.CreateInstanceIfNeed();
        
        _httpServer = new HttpServer(port ,OnResponseHandler);
        _httpServer.StartAsync();


#if UNITY_EDITOR
        Process.Start($"http://localhost:{port}");
#endif
    }
    
    private async Task<string> OnResponseHandler(HttpServerContext context)
    {
        string finalHtml = "";
        string absolutePath = context.AbsolutePath;
        
        
        Debug.Log(absolutePath);
        
        if (absolutePath.StartsWith("/hierarchy"))
        {
            await UnityCallbackUpdate.OnUpdateFromMainThreadAsync(() =>
            {
                lastData = HierarchyTools.GetHierarchyActiveScene();
                finalHtml = TreeHtmlMake.InsertCodeInHtml(lastData.rootNode, treeHtmlFile.text);
            });
        }
        else if (absolutePath.StartsWith("/id"))
        {
            string idStr = absolutePath.Replace("/id", "");
            StringBuilder sb = new StringBuilder();
            if (int.TryParse(idStr, out int idInt))
            {
                if (lastData.gameobjectsDictonary.TryGetValue(idInt , out  GameObject go))
                {
                    sb.AppendLine("<html>");
                    {
                        await UnityCallbackUpdate.OnUpdateFromMainThreadAsync(() =>
                        {
                            if (go != null)
                            {
                                CreateInformationStrings(sb, go);
                            }
                            else
                            {
                                sb.AppendLine($"<p>Gameobject is not found</p>");
                            }
                        });
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

        return finalHtml;
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
