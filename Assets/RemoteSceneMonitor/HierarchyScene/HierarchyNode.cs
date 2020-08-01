using Newtonsoft.Json;
using UnityEngine;

public class HierarchyNode
{
    public bool isScene;
    public bool isEnable;
    public string name;
    public int id;
    public int pId;
    
    [JsonIgnore]
    public GameObject gameObject;

    public HierarchyNode[] children;
}