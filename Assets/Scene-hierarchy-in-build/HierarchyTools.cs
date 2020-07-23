using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHierarchyData
{
    public Dictionary<int , GameObject> gameobjectsDictonary = new Dictionary<int, GameObject>();
    public HierarchyNode rootNode;
}

public class HierarchyTools
{
    public static SceneHierarchyData GetHierarchyActiveScene()
    {
        var activeScene = SceneManager.GetActiveScene();
        return GetHierarchyByScene(activeScene);
    }
    
    public static SceneHierarchyData GetHierarchyByScene(Scene scene)
    {
        SceneHierarchyData sceneHierarchyData = new SceneHierarchyData();
        
        var rootObjects = scene.GetRootGameObjects();
        HierarchyNode sceneNode = new HierarchyNode
        {
            isScene = true, 
            instanceId = -1,
            gameObject = null
        };
        
        GetHierarchy(sceneNode , rootObjects , sceneHierarchyData);
        sceneHierarchyData.rootNode = sceneNode;
        
        return sceneHierarchyData;
    }

    public static void GetHierarchy(HierarchyNode node , IEnumerable<GameObject> childGameObjects , SceneHierarchyData sceneHierarchyData)
    {
        List<HierarchyNode> childNodes = new List<HierarchyNode>();
        
        if (childGameObjects != null)
        {
            foreach (var children in childGameObjects)
            {
                sceneHierarchyData.gameobjectsDictonary[children.GetInstanceID()] = children;
                
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
                
                GetHierarchy(nodeChild , childChild , sceneHierarchyData);
                childNodes.Add(nodeChild);
            }
        }

        node.childrens = childNodes.ToArray();
    }
}