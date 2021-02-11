using UnityEditor;

namespace RemoteSceneMonitor.Scripts.Editor
{
    public static class PackingStreamingAssets
    {
        private const string PathToStreamingAssets = "Assets\\StreamingAssets\\RemoteSceneMonitorResources";
        private const string PathToOutPackageFile = "Assets\\RemoteSceneMonitor\\PackStreamingAssets\\PackStreamingAssets.data.txt";

        [MenuItem("PackingInOneFile/Packing streaming assets folder")]
        private static void PackingStreamingAssetsFolder()
        {
            PackingInOneFileEditor.PackingStreamingAssetsFolder(PathToStreamingAssets , PathToOutPackageFile);
        }
    }
}