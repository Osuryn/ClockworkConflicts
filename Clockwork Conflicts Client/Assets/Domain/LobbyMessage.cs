namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : LobbyMessage
    //========================================================================
    //- This class is  used when recieving or sending lobbymessages.
    //========================================================================
    class LobbyMessage : Message
    {
        #region Init

        //variables

        //constructor
        public LobbyMessage(int type, int senderID, string message = "")
        {
            this.type = type;
            this.senderID = senderID;
            this.message = message;
        }

        public LobbyMessage(int type, string message = "")
        {
            this.type = type;
            this.senderID = DomainController.getInstance().myAccount.accountId;
            this.message = message;
        }

        #endregion
    }
}
