namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : Server
    //========================================================================
    //- This class holds all info about a server
    //========================================================================
    public class Server
    {

        #region Init

        public int serverPort { get; private set; }
        public string serverName { get; private set; }
        public string serverStatus { get; private set; }
        public string serverIP { get; private set; }

        public Server(int serverPort, string servername, string serverstatus, string serverIP)
        {
            this.serverPort = serverPort;
            this.serverName = servername;
            this.serverStatus = serverstatus;
            this.serverIP = serverIP;
        }
        #endregion

    }
}
