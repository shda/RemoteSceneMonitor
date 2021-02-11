using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace RemoteSceneMonitor
{
    [CreateAssetMenu(menuName = "PackingInOneFile/" + nameof(PackingInOneFile))]
    public class PackingInOneFile : ScriptableObject
    {
        [SerializeField]
        private TextAsset packingDataAsset;

        private string textFromAsset;
        private PackingData _packingData;
        public PackingData PackingData
        {
            get
            {
                if (_packingData == null)
                {
                    lock (this)
                    {
                        _packingData = LoadingPackingData();
                    }
                }

                return _packingData;
            }
        }

        private void OnEnable()
        {
            textFromAsset = packingDataAsset.text;
            _packingData = null;
        }
        
        private PackingData LoadingPackingData()
        {
            var data = PackingDataSerializer.DeserializeFromString(textFromAsset);
            textFromAsset = "";
            return data;
        }

        public byte[] GetFileBytes(string fileName)
        {
            byte[] bytes = null;
            PackingData?.filesDictionary.TryGetValue(fileName, out bytes);
            return bytes;
        }
    }
}