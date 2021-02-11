using System.Collections.Generic;

namespace RemoteSceneMonitor
{
    [System.Serializable]
    public class PackingData
    {
        public Dictionary<string, byte[]> filesDictionary = new Dictionary<string, byte[]>();
    }
}