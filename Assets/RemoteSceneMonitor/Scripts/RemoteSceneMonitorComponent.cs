using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

namespace RemoteSceneMonitor
{
    public class RemoteSceneMonitorComponent : MonoBehaviour
    {
        [SerializeField]
        private int port = 1234;
        
        [Header("Debug")]
        [SerializeField]
        private bool openBrowserAfterPressPlayEditor = true;
        [SerializeField]
        private bool logToConsole = true;
        
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
            
            LogToConsoleConfig.IsLogToConsole = logToConsole;
            _remoteSceneMonitor = new RemoteSceneMonitor(port);
            
#if UNITY_EDITOR
            if (openBrowserAfterPressPlayEditor)
            {
                Process.Start($"http://localhost:{port}");
            }
#endif
            DontDestroyOnLoad(gameObject);
        }
        
        
        private void OnDestroy()
        {
            _remoteSceneMonitor?.Dispose();
        }
    }
}