using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonSouzM.ApiMonitor;
using Cysharp.Threading.Tasks;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class HierarchySceneNancyModule  : NancyModule
{
    public HierarchySceneNancyModule()
    {
        Get["/" , true] = async (x, y) =>
        {
            await UniTask.SwitchToMainThread();
            var node = HierarchyTools.GetHierarchyActiveScene();
            
            string text = JsonConvert.SerializeObject(node, Formatting.Indented );
            return text;
        };
        
        Get["/hierarchy" , true] = async (x, y) =>
        {
            await UniTask.SwitchToMainThread();
            var node = HierarchyTools.GetHierarchyActiveScene();
            string text = CreateHtmlTree(node);
            return text;
        };
    }

    private string CreateHtmlTree(HierarchyNode node)
    {
        StringBuilder finalHtml = new StringBuilder();
        finalHtml.Append("<ul>");
        {
            CreateHtmlNode(node , finalHtml);
        }
        finalHtml.Append("</ul>");

        finalHtml.AppendLine(ClickFunction());
        
        return finalHtml.ToString();
    }

    private void CreateHtmlNode(HierarchyNode node , StringBuilder sb)
    {
        StringBuilder nodeBuilder = new StringBuilder();

        string startLine = $"<li><span class=\"caret\">{node.name}</span>";
        if (node.gameObject == null || !node.gameObject.activeInHierarchy)
        {
            startLine = $"<li><span class=\"caret\" style=\"color:#AAAAAA\";>{node.name}</span>";
        }

        nodeBuilder.Append(startLine);
        {
            if (node.childrens.Any())
            {
                nodeBuilder.AppendLine("<ul class=\"nested\">");
                {
                    foreach (var hierarchyNode in node.childrens)
                    {
                        CreateHtmlNode(hierarchyNode , nodeBuilder);
                    }
                }
                nodeBuilder.AppendLine("</ul>");
            }
        }
        nodeBuilder.Append($"</li>");
        sb.AppendLine(nodeBuilder.ToString());
    }

    private string ClickFunction()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<script>");
        sb.AppendLine("var toggler = document.getElementsByClassName(\"caret\");");
        sb.AppendLine("var i;");
        sb.AppendLine("for (i = 0; i < toggler.length; i++) {");
        sb.AppendLine("toggler[i].addEventListener(\"click\", function() {");
        sb.AppendLine("this.parentElement.querySelector(\".nested\").classList.toggle(\"active\");");
        sb.AppendLine("this.classList.toggle(\"caret-down\");");
        sb.AppendLine("});");
        sb.AppendLine("</script>");

        return sb.ToString();
    }
}
