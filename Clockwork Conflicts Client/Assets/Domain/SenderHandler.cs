using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using MMTD_Client.Network;

namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : SenderHandler
    //========================================================================
    //- This is the superclass for all sender handlers
    //========================================================================
    class SenderHandler
    {
        
        #region Init
        
        public bool senderActive { get; set; }
        public int port { get; set; }
        public string senderName { get; set; }
        public IPAddress ipAd { get; set; }
        public Socket socket { get; set; }

        private Thread sendMessageThread;

        public SenderHandler(string senderName, string ipAddress, int port)
        {
            this.senderName = senderName;
            this.ipAd = IPAddress.Parse(ipAddress);
            this.port = port;
            senderActive = true;

            sendMessageThread = new Thread(new ThreadStart(SendMessageThread));
            sendMessageThread.IsBackground = true;
            sendMessageThread.Name = senderName;
            sendMessageThread.Start();  
        }

        #endregion

        #region Extra Functions

        public void SendMessage(Message message)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(ipAd, port));

            ClientState state = new ClientState();
            state.Client = socket;
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] data = asen.GetBytes(DomainController.getInstance().PrepareMessageToSend(message));
            //add prefix to data
            state.DataToSend = new byte[data.Length + 4];
            byte[] prefix = BitConverter.GetBytes(data.Length);
            //copy data size prefix
            Buffer.BlockCopy(prefix, 0, state.DataToSend, 0, prefix.Length);
            //copy the data
            Buffer.BlockCopy(data, 0, state.DataToSend, prefix.Length, data.Length);

            socket.BeginSend(state.DataToSend, 0, state.DataToSend.Length,
              SocketFlags.None, new AsyncCallback(ClientSendCallback), state);
            Thread.Sleep(50);
        }

        public void SendMessageThread()
        { 
            while (senderActive)
            {
                Message nextMessage = DomainController.getInstance().GetNextMessageFromQueue(senderName);
                if (nextMessage != null)
                {
                    SendMessage(nextMessage);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        private void ClientSendCallback(IAsyncResult ar)
        {
            ClientState state = (ClientState)ar.AsyncState;
            SocketError socketError;
            int sentData = state.Client.EndSend(ar, out socketError);

            if (socketError != SocketError.Success)
            {
                state.Client.Close();
                return;
            }

            state.DataSent += sentData;

            if (state.DataSent != state.DataToSend.Length)
            {   //not all data was sent
                state.Client.BeginSend(state.DataToSend, state.DataSent,
                  state.DataToSend.Length - state.DataSent, SocketFlags.None,
                    new AsyncCallback(ClientSendCallback), state);
            }
            else
            {   //all data was sent
                Console.WriteLine("All data was sent. Size: {0}",
                  state.DataToSend.Length);
            }
        }

        #endregion

    }
}
