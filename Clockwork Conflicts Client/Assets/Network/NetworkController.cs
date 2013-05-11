using System.Net.Sockets;

using MMTD_Client.Domain;

namespace MMTD_Client.Network
{
    //========================================================================
    //  			  CLASS DESCRIPTION : NetworkController
    //========================================================================
    //- Handles everything on the Network layer
    //========================================================================
    class NetworkController
    {

        #region Init

        private static NetworkController networkController;

        private LoginClient loginClient;
        private ListenerHandler chatReciever;
        private ListenerHandler lobbyReciever;
        private SenderHandler chatSender;
        private SenderHandler lobbySender;
        private Server Loginserver;
        private Server Chatserver;
        private Server Lobbyserver;

        public int loginServerPort { get; set; }
        public string loginServerIpAddress { get; set; }
        public string chatServerIpAddress { get; set; }
        public string lobbyServerIpAddress { get; set; }
        public int chatPort { get; set; }
        public int lobbyPort { get; set; }


        private NetworkController()
        {
            loginServerIpAddress = DomainController.getInstance().loginServer.serverIP;
            loginServerPort = DomainController.getInstance().loginServer.serverPort;
            loginClient = new LoginClient(loginServerIpAddress, loginServerPort);

            chatServerIpAddress = loginServerIpAddress;
            lobbyServerIpAddress = loginServerIpAddress;
            chatPort = 26801;
            lobbyPort = 26803;
        }

        public static NetworkController getInstance()
        {
            if (networkController == null)
            {
                networkController = new NetworkController();
            }
            return networkController;
        }

        #endregion

        #region Extra Functions

        public bool stopConnections()
        {
            chatReciever.runServer = false;
            lobbyReciever.runServer = false;
            chatReciever.socket.Close();
            lobbyReciever.socket.Close();
            return true;
        }

        public void SendLogin(string username, string password)
        {
            loginClient.SendLogin(username, password);
        }

        public void SendLogout()
        {
            loginClient.SendLogout();
        }

        public void StartConnections()
        {
            chatReciever = new ListenerHandler("ChatListener", chatServerIpAddress, chatPort + 1);
            lobbyReciever = new ListenerHandler("LobbyListener", lobbyServerIpAddress, lobbyPort + 1);
            chatSender = new SenderHandler("ChatSender", chatServerIpAddress, chatPort);
            lobbySender = new SenderHandler("LobbySender", lobbyServerIpAddress, lobbyPort);
        }

        #endregion

    }
}
