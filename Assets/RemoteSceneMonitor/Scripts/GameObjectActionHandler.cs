using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using RemoteSceneMonitor.HierarchyScene;
using UnityEngine;

namespace RemoteSceneMonitor
{
    public class GameObjectActionHandler
    {
        public async UniTask<byte[]> ActionRequestHandler(NameValueCollection queryString)
        {
            var actionType = queryString.Get("type");

            if (string.IsNullOrEmpty(actionType))
            {
                throw new Exception("Dont find tag \"type\" in query string");
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
                case "changeTransform":
                    finalArray = await ActionChangeTransform(queryString);
                    break;
            }

            return finalArray;
        }

        private async Task<byte[]> ActionChangeTransform(NameValueCollection queryString)
        {
            var vector = new Vector3(
                ParseFloatString(queryString.Get("x")),
                ParseFloatString(queryString.Get("y")),
                ParseFloatString(queryString.Get("z")));
            
            await UniTask.SwitchToMainThread();
            
            var idGameObject = int.Parse(queryString.Get("id"));
            var hierarchy = HierarchyTools.GetHierarchyActiveScene();
            hierarchy.gameobjectsDictonary.TryGetValue(idGameObject, out var changeGameObject);

            var fieldChange = queryString.Get("transformType");
            switch (fieldChange)
            {
                case "position":
                    changeGameObject.transform.position = vector;
                    break;
                case "rotation":
                    changeGameObject.transform.rotation = Quaternion.Euler(vector);
                    break;
                case "scale":
                    changeGameObject.transform.localScale = vector;
                    break;
            }
            
            return ResponseTools.CreateOkResponse();
        }

        private float ParseFloatString(string value)
        {
            value = value.Replace(".", ",");
            return float.Parse(value);
        }
        
        private async UniTask<byte[]> ActionDelete(NameValueCollection queryString)
        {
            var idString = queryString.Get("id");
            if (string.IsNullOrEmpty(idString))
            {
                throw new Exception("Dont find id in ActionDelete action");
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
                throw new Exception("Dont find idSource in ActionMove action");
            }
        
            if (string.IsNullOrEmpty(idDestinationString))
            {
                throw new Exception("Dont find idDestination in ActionMove action");
            }

            var idSourceInt = int.Parse(idSourceString);
            var idDestinationInt = int.Parse(idDestinationString);
            
            await MoveChildren(idSourceInt , idDestinationInt);
            
            return ResponseTools.CreateOkResponse();
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