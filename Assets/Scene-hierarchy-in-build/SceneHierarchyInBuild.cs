using System.Diagnostics;
using System.Threading.Tasks;
using Assets.AR_RPG.Scripts.Utils;
using Http;
using UnityEngine;

public class SceneHierarchyInBuild : MonoBehaviour
{
    [SerializeField]
    private TextAsset treeHtmlFile;
    
    //private NancyHost _nancyHost;

    private HttpServer _httpServer;
    
    private void Awake()
    {
        UnityCallbackUpdate.CreateInstanceIfNeed();
        
        _httpServer = new HttpServer(1234 ,OnResponseHandler);
        _httpServer.StartAsync();


#if UNITY_EDITOR
        Process.Start("http://localhost:1234/nancy/hierarchy/");
#endif
    }

    private async Task<string> OnResponseHandler(HttpServerContext arg)
    {
        string finalHtml = "";
        await UnityCallbackUpdate.OnUpdateFromMainThread(() =>
        {
            HierarchyNode node = HierarchyTools.GetHierarchyActiveScene();
            finalHtml = TreeHtmlMake.InsertCodeInHtml(node, treeHtmlFile.text);
        });
  
        return finalHtml;
    }


    private void OnDestroy()
    {
        _httpServer.Dispose();
        _httpServer = null;
    }
}
