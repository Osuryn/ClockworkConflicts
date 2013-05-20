using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using MMTD_Client.Domain;
using MMTD_Client.Gui;

namespace MMTD_Client.Network
{
    class ClientHomeHandler
    {
        private DomainController domainController;

        public bool isActive { get; set; }
        public UdpClient udpClient { get; set; }
        public Thread sendThread { get; set; }
        public Thread receiveThread { get; set; }
        public byte[] receive_byte_array { get; set; }
        public string received_data { get; set; }
        public IPEndPoint sendingEndPoint { get; set; }
        private IPEndPoint server;

        public ClientHomeHandler()
        {
            isActive = true;
            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sendingEndPoint = new IPEndPoint(IPAddress.Parse("193.190.225.147"), 27000);
            
            receiveThread = new Thread(new ThreadStart(ReceiveThread));
            receiveThread.IsBackground = true;
            receiveThread.Name = "HomeReceiveThread";
            receiveThread.Start();

            sendThread = new Thread(new ThreadStart(SendThread));
            sendThread.IsBackground = true;
            sendThread.Name = "HomeSendThread";
            sendThread.Start();

            domainController = DomainController.getInstance();
        }


        public void ReceiveThread()
        {
            while (isActive)
            {
                receive_byte_array = udpClient.Receive(ref server);
                received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                domainController.IncommingHomeQueue.Enqueue(received_data);
            }
        }

        public void SendThread()
        {
            while (isActive)
            {
                if (domainController.OutgoingHomeQueue.Count != 0)
                {
                    string dataToSend = domainController.OutgoingHomeQueue.Dequeue();
                    if (dataToSend != null)
                    {
                        try
                        {
                            byte[] arrayToSend = GetBytes(dataToSend);
                            udpClient.Send(arrayToSend, arrayToSend.Length, sendingEndPoint);
                        }
                        catch (Exception e)
                        {
                            GuiController.getInstance().UnityLog(e.ToString());
                        }
                    }
                }
            }
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
