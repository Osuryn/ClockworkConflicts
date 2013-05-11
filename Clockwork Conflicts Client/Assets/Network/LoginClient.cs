using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

using MMTD_Client.Domain;
using MMTD_Client.Gui;

namespace MMTD_Client.Network
{
    //========================================================================
    //  			  CLASS DESCRIPTION : LoginClient
    //========================================================================
    //- Handles connection with the loginServer
    //========================================================================
    class LoginClient
    {

        #region Init
        private int port;
        private IPAddress ipAdress;
        private Socket socket;
        private ServerState receiveState;
        private bool dataReceived;
        private bool loggingOut = false;
        private Byte[] data;
        private System.Timers.Timer conTimeOut = null;
        private bool timerElapsed = false;

        public LoginClient(string ipAdress, int port)
        {
            data = new byte[100];
            this.ipAdress = IPAddress.Parse(ipAdress);
            this.port = port;
            conTimeOut = new System.Timers.Timer(30000);
            conTimeOut.Elapsed += new ElapsedEventHandler(conTimeOut_Elapsed);
        }

        #endregion

        private void conTimeOut_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerElapsed = true;
            conTimeOut.Stop();
            conTimeOut = new System.Timers.Timer(30000);
        }

        #region Extra Functions

        public void SendLogin(string username, string password)
        {
            try
            {
                bool timeOut = false;
                dataReceived = false;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                GuiController.getInstance().SetDebugText("Connecting...");
                socket.Connect(new IPEndPoint(ipAdress, port));
                // use the ipaddress as in the server program

                receiveState = new ServerState();
                receiveState.Client = socket;

                GuiController.getInstance().SetDebugText("Connected... Checking login credentials");

                SendMessage(1, username, password);

                conTimeOut.Start();
                while (!dataReceived)
                {
                    if (timerElapsed)
                    {
                        timeOut = true;
                        break;
                    }
                    Thread.Sleep(50);
                }

                if (!timeOut)
                {

                    Console.Write(data[4]);

                    //Byte values
                    //***********************************
                    //255 =   "Login Succes"
                    //0   =   "Account does not exist"
                    //1   =   "Account blocked"
                    //2   =   "Account already logged in"
                    switch (data[4])
                    {
                        case 255:
                            if (data.Length >= 6)
                            {
                                string accountInfo = "";

                                for (int i = 5; i < data.Length; i++)
                                    accountInfo += (Convert.ToChar(data[i]));

                                DomainController.getInstance().ReceiveAccountInfo(accountInfo);

                                DomainController.getInstance().loggedIn = true;

                                if (DomainController.getInstance().myAccount.guildId != -1)
                                {
                                    DomainController.getInstance().AddLobbyMessageToQueue(32, DomainController.getInstance().myAccount.guildId.ToString());
                                }

                                //Update GUI
                                GuiController.getInstance().SetDebugText("Succesfully connected!");
                                GuiController.getInstance().UserLoggedIn();

                                NetworkController.getInstance().StartConnections();

                                //get Social lists
                                //GuiController.getInstance().friendsWindow.ClearSocialLists(1); //clear friends
                                DomainController.getInstance().AddLobbyMessageToQueue(2); //friends
                                //GuiController.getInstance().friendsWindow.ClearSocialLists(3); //clear pending
                                DomainController.getInstance().AddLobbyMessageToQueue(10); //pending friends

                                //get channels
                                DomainController.getInstance().AddLobbyMessageToQueue(20);
                            }
                            break;
                        case 0:
                            GuiController.getInstance().SetDebugText("No account found, check your username and password.");
                            break;
                        case 1:
                            GuiController.getInstance().SetDebugText("Can't connect, your account is blocked.");
                            break;
                        case 2:
                            GuiController.getInstance().SetDebugText("Account already logged in!");
                            break;
                        default:
                            GuiController.getInstance().SetDebugText("Received unknown byte value from server: " + data[0]);
                            break;
                    }
                }
                else
                {
                    GuiController.getInstance().SetDebugText("Timed out when trying to connect to server.");
                }

                socket.Close();
                Thread.Sleep(1000);

            }
            catch (Exception e)
            {
                GuiController.getInstance().SetDebugText("Couldn't connect to Loginserver..." + e.ToString());
            }
        }

        public void SendLogout()
        {
            SendMessage(0, DomainController.getInstance().myAccount.accountId + "", "");
        }

        private static bool checkResponse(Stream stm)
        {
            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);
            string output = "";
            for (int i = 0; i < k; i++)
                output += (Convert.ToChar(bb[i]));
            if (Convert.ToBoolean(output))
            {
                return true;
            }
            return false;
        }

        public void requestFriends()
        {
            dataReceived = false;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(ipAdress, port));
            // use the ipaddress as in the server program

            receiveState = new ServerState();
            receiveState.Client = socket;

            //send request message
            //SendMessage(1, username, password);

        }

        #endregion

        public void SendMessage(int type, string username, string password)
        {
            if (type == 0)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                GuiController.getInstance().SetDebugText("Connecting...");
                socket.Connect(new IPEndPoint(ipAdress, port));

                ClientState state = new ClientState();
                state.Client = socket;
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] data = asen.GetBytes("[" + type + "]" + username);
                //add prefix to data
                state.DataToSend = new byte[data.Length + 4];
                byte[] prefix = BitConverter.GetBytes(data.Length);
                //copy data size prefix
                Buffer.BlockCopy(prefix, 0, state.DataToSend, 0, prefix.Length);
                //copy the data
                Buffer.BlockCopy(data, 0, state.DataToSend, prefix.Length, data.Length);
                loggingOut = true;
                if (NetworkController.getInstance().stopConnections())
                {
                    socket.BeginSend(state.DataToSend, 0, state.DataToSend.Length,
                      SocketFlags.None, new AsyncCallback(ClientSendCallback), state);
                }
            }
            else
            {
                ClientState state = new ClientState();
                state.Client = socket;
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] data = asen.GetBytes("[" + type + "]" + username + "," + password);
                //add prefix to data
                state.DataToSend = new byte[data.Length + 4];
                byte[] prefix = BitConverter.GetBytes(data.Length);
                //copy data size prefix
                Buffer.BlockCopy(prefix, 0, state.DataToSend, 0, prefix.Length);
                //copy the data
                Buffer.BlockCopy(data, 0, state.DataToSend, prefix.Length, data.Length);

                socket.BeginSend(state.DataToSend, 0, state.DataToSend.Length,
                  SocketFlags.None, new AsyncCallback(ClientSendCallback), state);
            }

            //Thread.Sleep(50);
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
                if (!loggingOut)
                {
                    socket.BeginReceive(receiveState.Buffer, 0, receiveState.Buffer.Length, SocketFlags.None, new AsyncCallback(ServerReadCallback), receiveState);
                }
                else
                {
                    DomainController.getInstance().myAccount = null;
                    loggingOut = false;
                }
            }
        }

        private void ServerReadCallback(IAsyncResult ar)
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

                Console.WriteLine("Data received. Size: {0}", state.DataSize);
                Console.WriteLine("The Data is: " + output);
                Console.WriteLine("The Client is: " + client.LocalEndPoint);

                dataReceived = true;
                data = state.Buffer;

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
}
