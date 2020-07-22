using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommonSouzM.ApiMonitor;
using Cysharp.Threading.Tasks;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class HierarchySceneNancyModule  : NancyModule
{
    public HierarchySceneNancyModule()
    {
        Get["/" , true] = async (x, y) =>
        {
            await UniTask.SwitchToMainThread();
            var node = GetAllHierarchy();
            
            string text = JsonConvert.SerializeObject(node, Formatting.Indented );
            return text;
        };
    }
    public HierarchyNode GetAllHierarchy()
    {
        var activeScene = SceneManager.GetActiveScene();
        var rootObjects = activeScene.GetRootGameObjects();

        HierarchyNode sceneNode = new HierarchyNode
        {
            isScene = true, 
            instanceId = -1,
            gameObject = null
        };
        GetHierarchy(sceneNode , rootObjects);

        return sceneNode;
    }

    public void GetHierarchy(HierarchyNode node , IEnumerable<GameObject> childGameObjects)
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
