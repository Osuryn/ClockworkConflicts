using System;
using System.Collections;

using MMTD_Client.Gui;

namespace MMTD_Client.Domain
{
        //========================================================================
    //  			  CLASS DESCRIPTION : Friend
    //========================================================================
    //- This class holds a friends information for easy access
    //========================================================================
    public class Friend
    {

        #region Init

        public int friendId { get; set; }
        public string friendName { get; set; }
        public bool isOnline { get; set; }
        public bool pending { get; set; }
        public BitArray flags { get; private set; }

        public Friend(int friendId, byte flags, string friendName, bool pending = false, bool isOnline = false)
        {
            this.friendId = friendId;
            this.friendName = friendName;
            this.pending = pending;
            this.isOnline = isOnline;

            if (pending)
            { 
                //GuiController.getInstance().friendsWindow.AddPending(this); 
            }
            else
            {
                //GuiController.getInstance().friendsWindow.Addfriend(this);
            }

            byte[] array = new byte[1];
            array[0] = flags;
            this.flags = new BitArray(array);
        }

        public string GetOnlineText()
        {
            string onlineText = "";
            if (isOnline)
            {
                onlineText = "Online";
            }
            else
            {
                onlineText = "Offline";
            }
            return onlineText;
        }

        #endregion

    }
}
