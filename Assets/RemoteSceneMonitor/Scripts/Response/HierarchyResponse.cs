using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteSceneMonitor.HierarchyScene;
using RemoteSceneMonitor.Http;

namespace RemoteSceneMonitor
{
    public class HierarchyResponse : Response
    {
        private SceneHierarchyData _sceneHierarchyData;
        public HierarchyResponse(SceneHierarchyData sceneHierarchyData)
        {
            _sceneHierarchyData = sceneHierarchyData;
        }

        public override async Task<ResponseData> MakeResponseData()
        {
            if (_sceneHierarchyData == null)
            {
                _sceneHierarchyData = HierarchyTools.GetHierarchyActiveScene();
            }
            
            var json =  JsonConvert.SerializeObject(_sceneHierarchyData , Formatting.Indented);
            return new ResponseData()
            {
                data = ResponseTools.ConvertStringToResponseData(json)
            };
        }
    }
}