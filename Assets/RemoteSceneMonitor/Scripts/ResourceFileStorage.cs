using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteSceneMonitor
{
    public class ResourceFileStorage
    {
        private Dictionary<string , FileReadResult> _dictCatch = new Dictionary<string, FileReadResult>();
        private string _rootResourceFolder;
        private PackingInOneFile _packingInOneFile;

        public ResourceFileStorage(string rootResourceFolder, PackingInOneFile packingInOneFile)
        {
            _rootResourceFolder = rootResourceFolder;
            _packingInOneFile = packingInOneFile;
        }

        public async Task<FileReadResult> ReadFileFromResource(string fileName)
        {
            FileReadResult fileResult = null;
            
            if (_packingInOneFile != null)
            {
                var data = _packingInOneFile.GetFileBytes(fileName);
                fileResult = new FileReadResult {data = data, IsError = data == null};
            }
            else
            {
                string fullPath = _rootResourceFolder + "/" + fileName;

                fileResult = await ReadResourceFileUtils.ReadFileFromStreamingAssetsWithPlatform(fullPath);
                if (!fileResult.IsError)
                {
                    _dictCatch[fullPath] = fileResult;
                }
            }

            return fileResult;
        }
    }
}