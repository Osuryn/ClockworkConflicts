using System.Collections.Generic;

using MMTD_Client.Gui;

namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : Guild
    //========================================================================
    //- This class holds all information about a guild.
    //========================================================================
    public class Guild
    {
        public int guildId { get; set; }
        public string guildName { get; set; }
        public string guildTag { get; set; }
        public List<GuildMember> userList { get; set; }

        public Guild(int guildId, string guildName, string GuildTag, List<GuildMember> userList = null)
        {
            this.guildId = guildId;
            this.guildName = guildName;
            this.guildTag = guildTag;
            this.userList = userList;
            if (userList == null)
            {
                this.userList = new List<GuildMember>();
            }
            else 
            {
                this.userList = userList;
            }
        }

        public void AddMember(GuildMember member)
        {       
            userList.Add(member);
        }

        public void ClearMembers()
        {
            if (userList != null)
            {
                userList.Clear();
            }
        }
    }
}
