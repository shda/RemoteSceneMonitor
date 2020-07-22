using System;
using System.Net;
using System.Net.Sockets;

namespace CinemaLib.UdpClientServer
{
    public class UdpSender
    {
        public static void SendDataBroadcast(int port, byte[] sendData)
        {
            try
            {
                var sender = new UdpClient();
                sender.EnableBroadcast = true;
                sender.Send(sendData, sendData.Length, new IPEndPoint(IPAddress.Broadcast, port));
                sender.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendDataToUdpPort( string address, int port, byte[] sendData )
        {
            try
            {
                var sender = new UdpClient();
                sender.Send(sendData, sendData.Length, address , port);
                sender.Close();
            }
            catch ( Exception ex )
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}