namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : ChannelUser
    //========================================================================
    //- This class holds the data of a user in a chatchannel
    //========================================================================
    public class ChannelUser
    {

        #region Init

        public int userId { get; set; }
        public string userName { get; set; }

        public ChannelUser(int userId, string userName)
        {
            this.userId = userId;
            this.userName = userName;
        }

        #endregion

    }
}
