using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteSceneMonitor.HierarchyScene;
using RemoteSceneMonitor.Http;
using TaskLib;
using UnityEngine;

namespace RemoteSceneMonitor
{
    public class GameObjectInfoResponse : Response
    {
        private SceneHierarchyData _sceneHierarchyData;

        public GameObjectInfoResponse(SceneHierarchyData sceneHierarchyData)
        {
            _sceneHierarchyData = sceneHierarchyData;
        }

        public override async Task<ResponseData> MakeResponseData()
        {
            var idString = context.HttpListenerContext.Request.QueryString.Get("id");

            if (string.IsNullOrEmpty(idString))
            {
                throw new Exception("Dont find tag \"id\" in query string");
            }

            var idInt = int.Parse(idString);
            byte[] finalArray = new byte[0];
            
            if (_sceneHierarchyData == null)
            {
                _sceneHierarchyData = HierarchyTools.GetHierarchyActiveScene();
            }
            
            if (_sceneHierarchyData.gameobjectsDictonary.TryGetValue(idInt, out GameObject go))
            {
                await TaskSwitcher.SwitchToMainThread();
                
                Vector3 position = go.transform.position;
                Vector3 rotation = go.transform.rotation.eulerAngles;
                Vector3 scale = go.transform.localScale;


                GameObjectInfo objectInfo = new GameObjectInfo()
                {
                    activeSelf = go.activeSelf,
                    name = go.name,
                    position = position,
                    rotation = rotation,
                    scale = scale,
                };
                
                var json =  JsonConvert.SerializeObject(objectInfo , Formatting.Indented);
                finalArray = ResponseTools.ConvertStringToResponseData(json);

                if (LogToConsoleConfig.IsLogToConsole)
                {
                    Debug.Log(json);
                }
            }
            else
            {
                throw new Exception("Dont find id object " + idInt);
            }

            return new ResponseData()
            {
                data = finalArray,
            };
        }
    }
}