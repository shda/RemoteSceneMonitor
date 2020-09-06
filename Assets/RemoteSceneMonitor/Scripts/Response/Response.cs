using System.Threading.Tasks;
using RemoteSceneMonitor.HierarchyScene;
using RemoteSceneMonitor.Http;

namespace RemoteSceneMonitor
{
    public abstract class Response
    {
        protected HttpServerContext context;

        public void SetInfo(HttpServerContext context)
        {
            this.context = context;
        }

        public virtual async Task<ResponseData> MakeResponseData()
        {
            return null;
        }
    }
}