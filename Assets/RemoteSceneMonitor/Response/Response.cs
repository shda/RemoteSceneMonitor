using System.Threading.Tasks;
using RemoteSceneMonitor.HierarchyScene;
using RemoteSceneMonitor.Http;

namespace RemoteSceneMonitor
{
    public abstract class Response
    {
      //  protected SceneHierarchyData sceneHierarchyData;
        protected HttpServerContext context;

        public void SetInfo(HttpServerContext context)
        {
        //    this.sceneHierarchyData = sceneHierarchyData;
            this.context = context;
        }

        public virtual async Task<ResponseData> MakeResponseData()
        {
            return null;
        }
    }
}