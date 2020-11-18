using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteSceneMonitor
{
    public class ResourceFileStorage
    {
        private Dictionary<string , FileReadResult> _dictCatch = new Dictionary<string, FileReadResult>();
        private string _rootResourceFolder;

        public ResourceFileStorage(string rootResourceFolder)
        {
            _rootResourceFolder = rootResourceFolder;
        }

        public async Task<FileReadResult> ReadFileFromResource(string fileName)
        {
            string fullPath = _rootResourceFolder + "/" + fileName;

            var fileResult = await ReadResourceFileUtils.ReadFileFromStreamingAssetsWithPlatform(fullPath);
            if (!fileResult.IsError)
            {
                _dictCatch[fullPath] = fileResult;
            }

            return fileResult;
        }
    }
}