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
        private static RemoteSceneMonitorComponent instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            
            _remoteSceneMonitor = new RemoteSceneMonitor(port , updateDelay);
            
#if UNITY_EDITOR
            Process.Start($"http://localhost:{port}");
#endif
            DontDestroyOnLoad(gameObject);
        }
        
        
        private void OnDestroy()
        {
            _remoteSceneMonitor?.Dispose();
        }
    }
}