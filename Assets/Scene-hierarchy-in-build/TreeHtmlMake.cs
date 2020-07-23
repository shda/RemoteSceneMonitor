using System.Linq;
using System.Text;
using UnityEngine;

public static class TreeHtmlMake
{
    public static string InsertCodeInHtml(HierarchyNode rootNode , string templateHtml)
    {
        string treeHtml = CreateHtmlTree(rootNode);
        return  templateHtml.Replace("{treeview_insert}", treeHtml);
    }
    
    private static string CreateHtmlTree(HierarchyNode rootNode)
    {
        StringBuilder finalHtml = new StringBuilder();
        finalHtml.Append("<ul>");
        {
            CreateHtmlNode(rootNode , finalHtml);
        }
        
        finalHtml.Append("</ul>");
        return finalHtml.ToString();
    }

    private static void CreateHtmlNode(HierarchyNode node , StringBuilder sb)
    {
        StringBuilder nodeBuilder = new StringBuilder();

        bool isDisable = node.gameObject == null || !node.gameObject.activeInHierarchy;
        Color colorLine = Color.black;

        if (isDisable)
        {
            colorLine = Color.gray;
        }
 
        string  colorLineHtml = ColorUtility.ToHtmlStringRGB(colorLine);
        
        string nameObject = node.name;
        int id = node.instanceId;
        string startLine = $"<li>" +
                           $"<div>" +
                                $"<p>" +
                                    $"<a href=\"#\" class=\"sc\" onclick=\"return UnHide(this)\">&#9660;</a>" +
                                    $"<a onclick=\"return OpenId({id})\" style=\"color:#{colorLineHtml}\" >{nameObject}</a>" +
                                $"</p>" +
                           $"</div>";
        
        /*
        if (node.gameObject == null || !node.gameObject.activeInHierarchy)
        {
            startLine = $"<li><span class=\"caret\" style=\"color:#AAAAAA\";>{node.name}</span>";
        }
        */

        nodeBuilder.Append(startLine);
        {
            if (node.childrens.Any())
            {
                nodeBuilder.AppendLine("<ul class=\"cl\">");
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
}