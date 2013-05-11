using System;

//using System.Drawing;
//using System.Windows.Forms;

using MMTD_Client.Domain;

namespace MMTD_Client.Gui
{
    public partial class GuildWindow
    {
        
        #region Init

        public GuildWindow()
        {

        }

        #endregion
		/*
        #region Event Handlers

        private void btn_CreateGuild_Click(object sender, EventArgs e)
        {
            if (txt_GuildName.Text != "" && txt_GuildTag.Text != "")
            {
                DomainController.getInstance().AddLobbyMessageToQueue(30, txt_GuildName.Text + "|" + txt_GuildTag.Text);
            }
        }

        private void btn_Disband_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to disband the guild? This will delete the guild without any possible recovery.", "Disband Guild", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DomainController.getInstance().AddLobbyMessageToQueue(31, DomainController.getInstance().myGuild.guildId.ToString());
            }
        }

        private void btn_RemoveMember_Click(object sender, EventArgs e)
        {
            if (lvw_Members.SelectedItems.Count == 1)
            {
                if (MessageBox.Show("Are you sure you want to remove this member form the guild?", "Delete Member", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int id = DomainController.getInstance().GetGuildMemberIdByScreenName(lvw_Members.SelectedItems[0].Text);
                    if (id != -1)
                    {
                        DomainController.getInstance().AddLobbyMessageToQueue(34, id.ToString());
                    }
                }
            }
        }

        private void btn_JoinChat_Click(object sender, EventArgs e)
        {
            GuiController.getInstance().ShowChatWindow(GuiController.getInstance().guildChatChannel, true);
        }

        private void lvw_Members_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //don't do anything if nothing is selected
            if (lvw_Members.SelectedItems.Count != 0)
            {
                GuiController.getInstance().createConversation(lvw_Members.SelectedItems[0].SubItems[0].Text);
            }
        }

        #endregion
		 */
        #region Extra Functions

        public void UpdateRank(byte flags)
        {

        }

        public void UpdateTitle(string title)
        {

        }

        public void UpdateMemberList(GuildMember member)
        {

        }

        #endregion

    }
}
