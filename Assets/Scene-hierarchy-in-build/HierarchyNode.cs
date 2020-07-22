using UnityEngine;

public class HierarchyNode
{
    public bool isScene;
    public string name;
    public int instanceId;
    public GameObject gameObject;

    public HierarchyNode[] childrens;
}