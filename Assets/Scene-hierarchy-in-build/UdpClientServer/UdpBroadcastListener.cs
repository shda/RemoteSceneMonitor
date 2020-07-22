using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CinemaLib.Utils;
using UnityEngine;

namespace CinemaLib.UdpClientServer
{
    public class UdpBroadcastListener : IDisposable
    {
        private const int sleepThread = 10;
        private int port;
        public bool isWork { get; private set; }
        public event Action<UdpClient, IPAddress, byte[]> OnReceiveBytes;

        private UdpClient udpReceiver;

        public void StartListener(int port)
        {
            if (isWork)
            {
                StopForceServer();
            }

            this.port = port;
            isWork = true;

            ThreadsController.StartThread(WorkLoop);
        }

        public void StopServer()
        {
            isWork = false;
        }

        public void StopForceServer()
        {
            isWork = false;
            ThreadsController.StopThread(WorkLoop);
        }

        private void WorkLoop(object state)
        {
            udpReceiver = null;

            while (udpReceiver == null)
            {
                try
                {
                    udpReceiver = new UdpClient
                    {
                        ExclusiveAddressUse = false
                    };
                    udpReceiver.Client.SetSocketOption(SocketOptionLevel.Socket,
                        SocketOptionName.ReuseAddress, true);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    ThreadsController.Sleep(500);
                }
            }

            try
            {
                var iPEndPoint = new IPEndPoint(IPAddress.Any, port);

                udpReceiver.Client.Bind(iPEndPoint);

                var multicastAddress = IPAddress.Parse("239.0.0.222");
                udpReceiver.JoinMulticastGroup(multicastAddress);

                while (isWork)
                {
                    try
                    {
                        var clientRequestData = udpReceiver.Receive(ref iPEndPoint);
                        OnReceiveBytes?.Invoke(udpReceiver, iPEndPoint.Address, clientRequestData);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    ThreadsController.Sleep(sleepThread);
                }
            }
            catch ( ThreadInterruptedException)
            {

            }
            catch ( Exception ex )
            {
                Debug.LogException(ex);
            }


            udpReceiver.Close();
        }

        public void Dispose()
        {
            udpReceiver?.Close();
        }
    }
}