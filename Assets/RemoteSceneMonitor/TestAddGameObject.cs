using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddGameObject : MonoBehaviour
{
    public void OnAddGameObject()
    {
        GameObject newGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
}
