namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : Message
    //========================================================================
    //- This class is the Message, used when recieving or sending messages.
    //========================================================================
    public class Message
    {

        #region Init

        //variables
        public int type { get; protected set; }
        public int senderID { get; protected set; }
        public string message { get; protected set; }

        //constructor

        #endregion

    }
}
