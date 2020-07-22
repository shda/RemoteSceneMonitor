using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Nancy;
using UnityEngine;

public class HierarchySceneNancyModule  : NancyModule
{
    public static HierarchySceneNancyModule Instance { get; private set; }

    private static Dictionary<string , string> _htmlTemplates = new Dictionary<string, string>();
    
    public static void SetTemplates(Dictionary<string , string> htmlTemplates)
    {
        _htmlTemplates = htmlTemplates;
    }

    public HierarchySceneNancyModule()
    {
        Instance = this;
        
        Get["/" , true] = async (x, y) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<frameset cols=\"30%,70%\">");
            sb.AppendLine("<frame src=\"./hierarchy\" name=\"frame1\">");
            sb.AppendLine("<frame src=\"./hierarchy\" name=\"frame2\">");
            sb.AppendLine("</frameset>");
            sb.AppendLine("</html>");
            return sb.ToString();
        };
        
        Get["/hierarchy" , true] = async (x, y) =>
        {
            await UniTask.SwitchToMainThread();
            var node = HierarchyTools.GetHierarchyActiveScene();

            string html = TreeHtmlMake.InsertCodeInHtml(node, _htmlTemplates[TemplateFileNames.treeHtmlFile]);
            return html;
        };
        
        Get["/hierarchy/{id}" , true] = async (x, y) =>
        {
            await UniTask.SwitchToMainThread();
            var node = HierarchyTools.GetHierarchyActiveScene();

            string html = TreeHtmlMake.InsertCodeInHtml(node, _htmlTemplates[TemplateFileNames.treeHtmlFile]);

            return html;
        };
    }
}
