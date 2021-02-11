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
            string outString = "";

            using (var input = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(input, packingData);
                input.Seek(0, SeekOrigin.Begin);

                using (var output = new MemoryStream())
                {
                    using (var deflateStream = new DeflateStream(output, CompressionMode.Compress))
                    {
                        input.CopyTo(deflateStream);
                        deflateStream.Close();
                    }

                    outString = Convert.ToBase64String(output.ToArray());
                }
            }

            return outString;
        }

        public static PackingData DeserializeFromString(string text)
        {
            PackingData message = null;
            using (var input = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress))
                {
                    using (var output = new MemoryStream())
                    {
                        deflateStream.CopyTo(output);
                        deflateStream.Close();
                        output.Seek(0, SeekOrigin.Begin);

                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        message = (PackingData) binaryFormatter.Deserialize(output);
                    }
                }
            }

            return message;
        }
    }
}