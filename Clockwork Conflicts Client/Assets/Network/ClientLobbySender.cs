using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using MMTD_Client.Domain;

namespace MMTD_Client.Network
{
    //========================================================================
    //  			  CLASS DESCRIPTION : ClientLobbySender
    //========================================================================
    //- Handles the outgoing messages to the lobbyserver
    //========================================================================
    class ClientLobbySender
    {
        
        #region Init

        private int port;
        private Thread sendMessageThread;
        private Socket socket;
        private IPAddress ipAd;

        public ClientLobbySender(string ipAddress, int port)
        {
            this.ipAd = IPAddress.Parse(ipAddress);
            this.port = port;

            //sendMessageThread = new Thread(new ThreadStart(SendMessageThread));
            //sendMessageThread.IsBackground = true;
            //sendMessageThread.Name = "SendMessage";
            //sendMessageThread.Start();

            SendMessage(0);          
        }

        #endregion

        #region Extra Functions

        public void SendMessage(int type)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(ipAd, port));

            ClientState state = new ClientState();
            state.Client = socket;
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] data = asen.GetBytes("[" + type + "," + DomainController.getInstance().myAccount.accountId + "]");
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

        //public void SendMessageThread()
        //{ 
        //    while (senderActive)
        //    {
        //        Message nextMessage = DomainController.getInstance().GetNextMessageFromQueue();
        //        if (nextMessage != null)
        //        {
        //            SendMessage(nextMessage);
        //        }
        //    }
        //}

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
