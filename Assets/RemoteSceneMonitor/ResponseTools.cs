using System.Text;

namespace RemoteSceneMonitor
{
    public static class ResponseTools
    {
        public static byte[] CreateOkResponse()
        {
            return ConvertStringToResponseData("200");
        }

        public static byte[] ConvertStringToResponseData(string text)
        {
            return Encoding.UTF8.GetBytes(text);;
        }
    }
}