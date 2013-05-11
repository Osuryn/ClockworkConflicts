using System.Net.Sockets;

namespace MMTD_Client.Network
{
    //========================================================================
    //  			  CLASS DESCRIPTION : ClientState
    //========================================================================
    //- Stores the state of the client
    //========================================================================
    class ClientState
    {

        #region Init

        public int DataSent = 0; //data already sent
        public byte[] DataToSend; //data to be trasferred
        public Socket Client; //client socket

        #endregion

    }
}
