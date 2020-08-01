using System.Collections.Specialized;
using Cysharp.Threading.Tasks;

namespace RemoteSceneMonitor
{
    public class GameObjectActionHandler
    {
        public async UniTask<byte[]> ActionRequestHandler(NameValueCollection queryString)
        {
            var actionType = queryString.Get("type");

            if (string.IsNullOrEmpty(actionType))
            {
                return ResponseTools.ConvertStringToResponseData("Dont find tag \"type\" in query string");
            }

            byte[] finalArray = new byte[0];

            switch (actionType)
            {
                case "delete":
                    finalArray = await ActionDelete(queryString);
                    break;
                case "move":
                    finalArray = await ActionMove(queryString);
                    break;
            }

            return finalArray;
        }

        private async UniTask<byte[]> ActionDelete(NameValueCollection queryString)
        {
            var idString = queryString.Get("id");
            if (string.IsNullOrEmpty(idString))
            {
                return ResponseTools.ConvertStringToResponseData("Dont find id in ActionDelete action");
            }

            if (int.TryParse(idString, out var idDelete))
            {
                await DeleteGameObjectById(idDelete);
                return ResponseTools.CreateOkResponse();
            }
        
            return new byte[0];
        }
    
        private async UniTask<byte[]> ActionMove(NameValueCollection queryString)
        {
            var idSourceString = queryString.Get("idSource");
            var idDestinationString = queryString.Get("idDestination");
        
            if (string.IsNullOrEmpty(idSourceString))
            {
                return ResponseTools.ConvertStringToResponseData("Dont find idSource in ActionMove action");
            }
        
            if (string.IsNullOrEmpty(idDestinationString))
            {
                return ResponseTools.ConvertStringToResponseData("Dont find idDestination in ActionMove action");
            }

            if (int.TryParse(idSourceString, out var idSourceInt) &&
                int.TryParse(idDestinationString, out var idDestinationInt))
            {
                await MoveChildren(idSourceInt , idDestinationInt);
                return ResponseTools.CreateOkResponse();
            }
        
            return new byte[0];
        }
    
        private async UniTask DeleteGameObjectById(int idDeleteObject)
        {
            await UniTask.SwitchToMainThread();
        
            var hierarchy = HierarchyTools.GetHierarchyActiveScene();
            hierarchy.gameobjectsDictonary.TryGetValue(idDeleteObject, out var deleteObject);
            if (deleteObject != null)
            {
                UnityEngine.Object.DestroyImmediate(deleteObject.gameObject);
            }
        }

        private async UniTask MoveChildren(int idMove, int idTarget)
        {
            await UniTask.SwitchToMainThread();
        
            var hierarchy = HierarchyTools.GetHierarchyActiveScene();

            hierarchy.gameobjectsDictonary.TryGetValue(idMove, out var moveObject);
            hierarchy.gameobjectsDictonary.TryGetValue(idTarget, out var targetObject);
        
        
            if(moveObject != null)
            {
                if (targetObject != null)
                {
                    moveObject.transform.parent = targetObject.transform;
                }
                else
                {
                    moveObject.transform.parent = null;
                }
            }
        }
    }
}