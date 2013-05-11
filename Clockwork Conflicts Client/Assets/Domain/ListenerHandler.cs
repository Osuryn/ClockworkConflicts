using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using MMTD_Client.Gui;
using MMTD_Client.Network;

namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : ReceiverHandler
    //========================================================================
    //- This is the superclass for all receiver handlers
    //========================================================================
    class ListenerHandler
    {

        #region Init

        public int port { get; set; }
        public string listenerName { get; set; }
        public IPAddress ipAd { get; set; }
        public Socket socket { get; set; }
        public bool runServer { get; set; }

        Thread receiveMessageThread;

        public ListenerHandler(string listenerName, string ipAddress, int port)
        {
            runServer = true;
            this.listenerName = listenerName;
            ipAd = IPAddress.Parse(ipAddress);
            this.port = port;

            SendInitialMessage();

            receiveMessageThread = new Thread(new ThreadStart(ReceiveMessageThread));
            receiveMessageThread.IsBackground = true;
            receiveMessageThread.Name = listenerName;
            receiveMessageThread.Start();
        }

        #endregion

        #region Extra Functions

        public void ReceiveMessageThread()
        {
            while (runServer)
            {
                try
                {
                    ServerState state2 = new ServerState();

                    state2.Client = socket;
                    socket.BeginReceive(state2.Buffer, 0, state2.Buffer.Length, SocketFlags.None, new AsyncCallback(ServerReadCallback), state2);
                    Thread.Sleep(50);
                }
                catch (SocketException socketEx)
                {
#if DEBUG
                    GuiController.getInstance().ShowMessageBox(socketEx.ToString());
#endif
                }
            }
        }

        public void SendInitialMessage()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(ipAd, port));

            ClientState state = new ClientState();
            state.Client = socket;
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] data = asen.GetBytes("[" + DomainController.getInstance().myAccount.accountId + "]" + "Connected");
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

        private void ServerReadCallback(IAsyncResult ar)
        {
            try
            {
                if (runServer)
                {
                    ServerState state = (ServerState)ar.AsyncState;
                    Socket client = state.Client;
                    SocketError socketError;

                    int dataRead = client.EndReceive(ar, out socketError);
                    int dataOffset = 0; //to simplify logic

                    if (socketError != SocketError.Success)
                    {
                        client.Close();
                        return;
                    }

                    if (dataRead <= 0)
                    { //connection reset 
                        //client.Close();
                        return;
                    }

                    if (!state.DataSizeReceived)
                    {
                        if (dataRead >= 4)
                        {   //we received data size prefix
                            state.DataSize = BitConverter.ToInt32(state.Buffer, 0);
                            state.DataSizeReceived = true;
                            dataRead -= 4;
                            dataOffset += 4;
                        }
                    }

                    if ((state.Data.Length + dataRead) == state.DataSize)
                    {   //we have all the data
                        state.Data.Write(state.Buffer, dataOffset, dataRead);

                        string output = "";

                        for (int i = 4; i < state.DataSize + 4; i++)
                            output += (Convert.ToChar(state.Buffer[i]));

                        //GuiController.getInstance().SetDebugText(output);
						if (listenerName.ToLower() == "lobbylistener") 
						{
							DomainController.getInstance().IncommingLobbyMessage.Enqueue(output);
						}
						else if (listenerName.ToLower() == "chatlistener") 
						{
							DomainController.getInstance().IncommingChatMessage.Enqueue(output);
						}
                        //DomainController.getInstance().GetMessage(listenerName, output);
                        Console.WriteLine("Data received. Size: {0}", state.DataSize);
                        Console.WriteLine("The Data is: " + output);
                        Console.WriteLine("The Client is: " + client.LocalEndPoint);

                        return;
                    }
                    else
                    {
                        //there is still data pending, store what we've
                        //received and issue another BeginReceive
                        state.Data.Write(state.Buffer, dataOffset, dataRead);
                        client.BeginReceive(state.Buffer, 0, state.Buffer.Length,
                        SocketFlags.None, new AsyncCallback(ServerReadCallback), state);
                    }
                }
            }
            catch (Exception socketEx)
            {
#if DEBUG
                GuiController.getInstance().ShowMessageBox(socketEx.ToString());
#endif
            }
        }

        #endregion

    }
}
