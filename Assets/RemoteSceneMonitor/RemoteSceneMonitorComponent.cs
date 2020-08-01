using System.Diagnostics;
using UnityEngine;

namespace RemoteSceneMonitor
{
    public class RemoteSceneMonitorComponent : MonoBehaviour
    {
        [SerializeField]
        private int port = 1234;
        [SerializeField]
        private float updateDelay = 0.2f;
        
        private RemoteSceneMonitor _remoteSceneMonitor;


        private void Awake()
        {
            _remoteSceneMonitor = new RemoteSceneMonitor();
            
#if UNITY_EDITOR
            Process.Start($"http://localhost:{port}");
#endif
        }
        
        private void OnDestroy()
        {
            _remoteSceneMonitor?.Dispose();
        }
    }
}