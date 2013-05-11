using System.Collections.Generic;

using MMTD_Client.Gui;

namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : ChatChannel
    //========================================================================
    //- This class holds all the information of a chatchannel
    //========================================================================
    public class ChatChannel
    {

        #region Init

        //variables
        public int channelId { get; private set; }
        public string channelName { get; set; }
        public int ownerId { get; set; }
        public List<int> userList { get; set; }
        public bool joined { get; set; }
        public Queue<string> reveivedText { get; set; }
		
        public ChatChannel(bool joined, int channelId, string channelName, int ownerId = -1,  List<int> userList = null)
        {
            this.joined = joined;
            this.channelId = channelId;
            this.channelName = channelName;
            this.ownerId = ownerId;
            this.userList = userList;
            reveivedText = new Queue<string>();
        }

        #endregion

        #region Extra Functions


        #endregion
    }
}
