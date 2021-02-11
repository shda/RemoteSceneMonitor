using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace RemoteSceneMonitor
{
    public static class PackingDataSerializer
    {
        public static string SerializeToString(PackingData packingData)
        {
            using MemoryStream input = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(input, packingData);
            input.Seek(0, SeekOrigin.Begin);

            using MemoryStream output = new MemoryStream();
            using DeflateStream deflateStream = new DeflateStream(output, CompressionMode.Compress);
            input.CopyTo(deflateStream);
            deflateStream.Close();
            return Convert.ToBase64String(output.ToArray());
        }
        
        public static PackingData DeserializeFromString(string text)
        {
            using MemoryStream input = new MemoryStream(Convert.FromBase64String(text));
            using DeflateStream deflateStream = new DeflateStream(input, CompressionMode.Decompress);
            using MemoryStream output = new MemoryStream();
            
            deflateStream.CopyTo(output);
            deflateStream.Close();
            output.Seek(0, SeekOrigin.Begin);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            PackingData message = (PackingData)binaryFormatter.Deserialize(output);
            return message;
        }
    }
}