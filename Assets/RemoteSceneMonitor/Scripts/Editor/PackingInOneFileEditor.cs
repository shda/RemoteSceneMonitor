using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace RemoteSceneMonitor.Scripts.Editor
{
    public static class PackingInOneFileEditor
    {
        public static void PackingStreamingAssetsFolder(string pathToSourceFolder, string pathToOutPackingFile)
        {
            List<string> filesInDirs = new List<string>();
            GetAllFileInFolderRecursive(filesInDirs, pathToSourceFolder, new[] {".meta"});

            foreach (var filesInDir in filesInDirs)
            {
                Debug.Log(filesInDir);
            }

            PackingData packingData = new PackingData();

            foreach (var filePath in filesInDirs)
            {
                string finalPath = filePath.Replace(pathToSourceFolder + "\\", "").Replace("\\", "/");
                packingData.filesDictionary[finalPath] = File.ReadAllBytes(filePath);
            }

            string outString = PackingDataSerializer.SerializeToString(packingData);
            File.WriteAllText(pathToOutPackingFile , outString);
            
            AssetDatabase.Refresh();
        }

        private static void GetAllFileInFolderRecursive(List<string> allFiles, string path, string[] excludeExt)
        {
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                if (!excludeExt.Contains(ext))
                {
                    allFiles.Add(file);
                }
            }

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                GetAllFileInFolderRecursive(allFiles, directory, excludeExt);
            }
        }
    }
}