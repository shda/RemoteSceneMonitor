using System.Threading.Tasks;
using RemoteSceneMonitor.Http;
using UnityEngine;

namespace RemoteSceneMonitor
{
    public class FileGetResponse : Response
    {
        private ResourceFileStorage _resourceFileStorage;

        public FileGetResponse(ResourceFileStorage resourceFileStorage)
        {
            _resourceFileStorage = resourceFileStorage;
        }

        public override async Task<ResponseData> MakeResponseData()
        {
            var pathWithoutParams = context.AbsolutePath;
            ResponseData responseData = new ResponseData();

            string filePath = pathWithoutParams.Remove(0, 1);
            
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = "index.html";
            }
            
            FileReadResult fileReadResult = await _resourceFileStorage.ReadFileFromResource(filePath);

            if (!fileReadResult.IsError)
            {
                responseData.data = fileReadResult.data;
            }
            else
            {
                Debug.LogError($"Error load file - {filePath}");
                responseData.data = ResponseTools.ConvertStringToResponseData($"Error load file {filePath}");
            }

            return responseData;
        }
    }
}