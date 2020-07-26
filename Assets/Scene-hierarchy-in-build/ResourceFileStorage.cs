using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class ResourceFileStorage
{
    private Dictionary<string , string> _dictCatch = new Dictionary<string, string>();
    private string _rootResourceFolder;

    public ResourceFileStorage(string rootResourceFolder)
    {
        _rootResourceFolder = rootResourceFolder;
    }

    public async UniTask<FileReadResult> ReadFileFromResource(string fileName)
    {
        string fullPath = _rootResourceFolder + "/" + fileName;
        
        if (_dictCatch.ContainsKey(fullPath))
        {
            return new FileReadResult()
            {
                text = _dictCatch[fullPath],
            };
        }
        
        var fileResult = await ReadResourceFileUtils.ReadFileFromStreamingAssetsWithPlatform(fullPath);
        if (!fileResult.IsError)
        {
            _dictCatch[fullPath] = fileResult.text;
        }

        return fileResult;
    }
}