using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace RemoteSceneMonitor
{
    public class ReadResourceFileUtils
    {
        private static string GetPlatformPathToStreamingAssets(string path)
        {
            string finalPath = Path.Combine(Application.streamingAssetsPath, path);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                finalPath = "file://" + finalPath;
            }

            return finalPath;
        }
    
        public static async Task<FileReadResult> ReadFileFromStreamingAssetsWithPlatform(string fileName)
        {
            await UniTask.SwitchToMainThread();
        
            var finalPath = GetPlatformPathToStreamingAssets(fileName);
            var fileReadResult = new FileReadResult();
        
            UnityWebRequest loadFile = null;
            try
            {
                loadFile =  UnityWebRequest.Get(finalPath);
                await loadFile.SendWebRequest();
            }
            catch (Exception e)
            {
                fileReadResult.IsError = true;
                Debug.LogError($"UnityWebRequest {fileName}");
            }

            if (loadFile != null && loadFile.isDone)
            {
                if (loadFile.isNetworkError || loadFile.isHttpError)
                {
                    fileReadResult = new FileReadResult()
                    {
                        error = loadFile.error,
                        IsError = true,
                    };
                }
                else
                {
                    fileReadResult = new FileReadResult()
                    {
                        text = loadFile.downloadHandler.text,
                        data = loadFile.downloadHandler.data,
                    };
                }
            }

            return fileReadResult;
        }
    }
}