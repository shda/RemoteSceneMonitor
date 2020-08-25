using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemoteSceneMonitor.HierarchyScene
{
    public class SceneHierarchyData
    {
        [JsonIgnore]
        public Dictionary<int , GameObject> gameobjectsDictonary = new Dictionary<int, GameObject>();
        public HierarchyNode[] scenesRootNodesList;
    }
    
    public class HierarchyTools
    {
        public static SceneHierarchyData GetHierarchyActiveScene()
        {
            var allGameObjects = new Dictionary<int, GameObject>();
            var listRootScenes = new List<HierarchyNode>(SceneManager.sceneCount);
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var sceneNode = GetHierarchyByScene(scene , allGameObjects);
                listRootScenes.Add(sceneNode);
            }

            return new SceneHierarchyData()
            {
                gameobjectsDictonary = allGameObjects,
                scenesRootNodesList = listRootScenes.ToArray(),
            };
        }

        private static HierarchyNode GetHierarchyByScene(Scene scene , Dictionary<int , GameObject> dictObjects)
        {
           // SceneHierarchyData sceneHierarchyData = new SceneHierarchyData();
        
            var rootObjects = scene.GetRootGameObjects();
            HierarchyNode sceneNode = new HierarchyNode
            {
                isScene = true, 
                id = 0,
                pId = -1,
                gameObject = null,
                name = scene.name,
                isEnable = true,
            };
        
            GetHierarchy(sceneNode , rootObjects , dictObjects);
            //sceneHierarchyData.sceneNodes = sceneNode;
        
            return sceneNode;
        }

        private static void GetHierarchy(HierarchyNode node , IEnumerable<GameObject> childGameObjects , Dictionary<int , GameObject> dictObjects)
        {
            List<HierarchyNode> childNodes = new List<HierarchyNode>();
        
            if (childGameObjects != null)
            {
                foreach (var children in childGameObjects)
                {
                    dictObjects[children.GetInstanceID()] = children;
                
                    HierarchyNode nodeChild = new HierarchyNode()
                    {
                        name = children.name,
                        gameObject = children.gameObject,
                        id = children.GetInstanceID(),
                        pId = node.id,
                        isScene = false,
                        isEnable = children.gameObject.activeSelf,
                    };
                
                    List<GameObject> childChild = new List<GameObject>();

                    for (int i = 0; i < children.transform.childCount; i++)
                    {
                        var childTransform = children.transform.GetChild(i);
                        childChild.Add(childTransform.gameObject);   
                    }
                
                    GetHierarchy(nodeChild , childChild , dictObjects);
                    childNodes.Add(nodeChild);
                }
            }

            if (childNodes.Any())
            {
                node.children = childNodes.ToArray();
            }
            else
            {
                node.children = null;
            }
        }
    }
}