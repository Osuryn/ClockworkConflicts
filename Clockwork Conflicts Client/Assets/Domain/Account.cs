using System;
using System.Collections;

using MMTD_Client.Gui;

namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : Account
    //========================================================================
    //- This class holds all the information of a single user
    //- A new instance of this class is created when a user is pulled from the
    //  database
    //========================================================================
    public class Account
    {

        #region Init

        //variables
        public int accountId { get; private set; }
        public BitArray flags { get; private set; }
        public string screenName { get; set; }
        public int guildId { get; set; }
        public byte guildFlags { get; set; }
        public int partyID { get; set; }

        public Account(int accountId, byte flags, string screenName, int guildId)
        {
            this.accountId = accountId;
            
            byte[] array = new byte[1];
            array[0] = Convert.ToByte(flags);
            this.flags = new BitArray(array); 
            
            this.screenName = screenName;
            this.guildId = guildId;
            partyID = -1;
        }

        #endregion

        #region Extra Functions

        public void SetGuildFlags(byte flags)
        {
            guildFlags = flags;
        }

        #endregion

    }
}
