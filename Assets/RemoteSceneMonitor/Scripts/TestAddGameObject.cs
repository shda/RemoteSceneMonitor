using UnityEngine;

namespace RemoteSceneMonitor
{
    public class TestAddGameObject : MonoBehaviour
    {
        public void OnAddGameObject()
        {
            GameObject newGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
    }
}
