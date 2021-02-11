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

        private byte[] _bytes;
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
            _bytes = packingDataAsset.bytes;
            _packingData = null;
        }
        
        private PackingData LoadingPackingData()
        {
            PackingData packingData = null;
            MemoryStream memoryStream = new MemoryStream(_bytes);
 
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                packingData = (PackingData) formatter.Deserialize(memoryStream);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to deserialize. Reason: " + e.Message);
            }
            finally
            {
                memoryStream.Close();
            }

            return packingData;
        }

        public byte[] GetFileBytes(string fileName)
        {
            byte[] bytes = null;
            PackingData?.filesDictionary.TryGetValue(fileName, out bytes);
            return bytes;
        }
    }
}