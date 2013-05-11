using System;

//using System.Drawing;
//using System.Windows.Forms;

using MMTD_Client.Domain;

namespace MMTD_Client.Gui
{
    public partial class ChannelWindow
    {

        #region Init

        //private Size maximizedSize;
        //private Size minimizedSize { get; set; }

        public bool maximized { get; set; }

        public ChannelWindow()
        {

        }

        #endregion
		/*
        #region Event Handlers

        private void lvw_Channels_DoubleClick(object sender, EventArgs e)
        {
            ChatChannel channel = DomainController.getInstance().GetChannelByName(lvw_Channels.SelectedItems[0].Text);
            if (channel != null)
            {
                if (channel.chatWindow == null)
                {
                    DomainController.getInstance().AddLobbyMessageToQueue(22, channel.channelId.ToString());//join channel
                    //DomainController.getInstance().AddLobbyMessageToQueue(23, channel.channelId.ToString());//request channel details
                    channel.createWindow();
                }
                else
                {
                    GuiController.getInstance().ShowChatWindow(channel.chatWindow, false);
                }
            }
            else
            {
#if DEBUG
                MessageBox.Show("No channel found with name: " + lvw_Channels.SelectedItems[0].Text);
#endif
            }
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            if (maximized)
            {
                this.Size = minimizedSize;
                this.Location = new Point(this.Location.X, this.Location.Y + maximizedSize.Height - minimizedSize.Height);
                lvw_Channels.Hide();
                hideButton.Text = "^";
                maximized = false;
            }
            else
            {
                this.Size = maximizedSize;
                this.Location = new Point(this.Location.X, this.Location.Y - maximizedSize.Height + minimizedSize.Height);
                lvw_Channels.Show();
                hideButton.Text = "V";
                maximized = true;
            }
        }

        #endregion
		 */
        #region Extra Functions

        public void AddChannel(ChatChannel channel)
        {

        }

        public void ClearChannelList()
        {

        }

        #endregion

    }
}
