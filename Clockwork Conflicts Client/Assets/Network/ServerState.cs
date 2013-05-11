using System.IO;
using System.Net.Sockets;

namespace MMTD_Client.Network
{
    //========================================================================
    //  			  CLASS DESCRIPTION : ServerState
    //========================================================================
    //- Stores the state of a server
    //========================================================================
    class ServerState
    {

        #region Init

        public int DataSize = 0; //data size to be received by the server
        public byte[] Buffer = new byte[512]; //buffer for network i/o
        public bool DataSizeReceived = false; //whether prefix was received
        public MemoryStream Data = new MemoryStream(); //place where data is stored
        public Socket Client;   //client socket

        #endregion

    }
}
