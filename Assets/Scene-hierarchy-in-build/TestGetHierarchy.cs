using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;


public class HierarchyNode
{
    public bool isScene;
    public string name;
    public int instanceId;
    [JsonIgnore]
    public GameObject gameObject;

    public HierarchyNode[] childrens;
}


public class TestGetHierarchy : MonoBehaviour
{
    private void Awake()
    {
        var rootObjects = gameObject.scene.GetRootGameObjects();
        
        HierarchyNode sceneNode = new HierarchyNode();
        sceneNode.isScene = true;
        sceneNode.instanceId = -1;
        sceneNode.gameObject = null;
        GetHierarchy(sceneNode , rootObjects);
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
