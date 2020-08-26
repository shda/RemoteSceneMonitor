using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using RemoteSceneMonitor.HierarchyScene;
using RemoteSceneMonitor.Http;

namespace RemoteSceneMonitor
{
    public class ResponseFactory
    {
        private const string RootResourceFolder = "RemoteSceneMonitorResources";
        
        private SceneHierarchyData _sceneHierarchyData;
        private HttpServerContext _context;
        private ResourceFileStorage _resourceFileStorage;

        public ResponseFactory()
        {
            _resourceFileStorage = new ResourceFileStorage(RootResourceFolder);
        }
        
        public void SetData(HttpServerContext context)
        {
            _context = context;
        }

        public async Task<Response> CreateResponse(string pathWithoutParams)
        {
            Response response = null;
            
            if(pathWithoutParams.StartsWith("/json/hierarchy"))
            {
                await UniTask.SwitchToMainThread();
                _sceneHierarchyData = HierarchyTools.GetHierarchyActiveScene();
                response = new HierarchyResponse(_sceneHierarchyData);
            }
            else if(pathWithoutParams.StartsWith("/action"))
            {
                response = new ActionResponse();
            }
            else if (pathWithoutParams.StartsWith("/gameObjectInfo"))
            {
                response = new GameObjectInfoResponse(_sceneHierarchyData);
            }
            else
            {
                response = new FileGetResponse(_resourceFileStorage);
            }
            
            response?.SetInfo(_context);
            return response;
        }
    }
}