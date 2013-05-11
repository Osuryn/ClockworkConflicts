namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : ChatMessage
    //========================================================================
    //- This class is used when recieving or sending chatmessages.
    //========================================================================
    class ChatMessage : Message
    {
        #region Init

        //variables
        public int recieverID { get; private set; }
        public string date { get; private set; }

        //constructor
        public ChatMessage(string date, int senderID, int recieverID, int type, string message)
        {
            this.date = date;
            this.type = type;
            this.senderID = senderID;
            this.recieverID = recieverID;
            this.message = message;
        }

        #endregion
    }
}
