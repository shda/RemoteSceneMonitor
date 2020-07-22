using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HierarchyTools
{
    public static HierarchyNode GetHierarchyActiveScene()
    {
        var activeScene = SceneManager.GetActiveScene();
        return GetHierarchyByScene(activeScene);
    }
    
    public static HierarchyNode GetHierarchyByScene(Scene scene)
    {
        var rootObjects = scene.GetRootGameObjects();

        HierarchyNode sceneNode = new HierarchyNode
        {
            isScene = true, 
            instanceId = -1,
            gameObject = null
        };
        GetHierarchy(sceneNode , rootObjects);

        return sceneNode;
    }

    public static void GetHierarchy(HierarchyNode node , IEnumerable<GameObject> childGameObjects)
    {
        List<HierarchyNode> childNodes = new List<HierarchyNode>();
        
        if (childGameObjects != null)
        {
            foreach (var children in childGameObjects)
            {
                HierarchyNode nodeChild = new HierarchyNode()
                {
                    name = children.name,
                    gameObject = children.gameObject,
                    instanceId = children.GetInstanceID(),
                    isScene = false,
                };
                
                List<GameObject> childChild = new List<GameObject>();

                for (int i = 0; i < children.transform.childCount; i++)
                {
                    var childTransform = children.transform.GetChild(i);
                    childChild.Add(childTransform.gameObject);   
                }
                
                GetHierarchy(nodeChild , childChild);
                
                childNodes.Add(nodeChild);
            }
        }

        node.childrens = childNodes.ToArray();
    }
}