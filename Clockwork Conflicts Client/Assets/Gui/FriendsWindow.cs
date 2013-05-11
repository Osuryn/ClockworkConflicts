using System;
using System.ComponentModel;

//using System.Drawing;
//using System.Windows.Forms;

using MMTD_Client.Domain;

namespace MMTD_Client.Gui
{
    public partial class FriendsWindow
    {
		
        #region Init

        //private Size maximizedSize;
        //private Size minimizedSize { get; set; }

        public bool maximized { get; set; }

        public FriendsWindow()
        {

        }

        #endregion
		/*
        #region Event Handlers

        private void btnAdd_Click(object sender, EventArgs e)
        {
            switch (btnAdd.Text)
            {
                case "Add Friend":
                case "Add Ignore":
                    GuiController.getInstance().AddUser(btnAdd.Text);
                    break;
                case "Accept Friend":
                    if (lvw_Pending.SelectedItems.Count == 1)
                    {
                        int pendingId = DomainController.getInstance().getFriendIdByName(lvw_Pending.SelectedItems[0].SubItems[0].Text);
                        DomainController.getInstance().AddLobbyMessageToQueue(7, pendingId.ToString());
                    }
                    break;
                default:
                    break;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            switch (btnRemove.Text)
            {
                case "Remove Friend":
                    int friendId = DomainController.getInstance().getFriendIdByName(lvw_Friends.SelectedItems[0].SubItems[0].Text);
                    DomainController.getInstance().AddLobbyMessageToQueue(9, friendId.ToString());
                    break;
                case "Remove Ignore":
                    //not implemented on server-side
                    int removeId = DomainController.getInstance().getFriendIdByName(lvw_Ignore.SelectedItems[0].SubItems[0].Text);
                    DomainController.getInstance().AddLobbyMessageToQueue(17, removeId.ToString());
                    break;
                case "Remove Request":
                    int pendingId = DomainController.getInstance().getFriendIdByName(lvw_Pending.SelectedItems[0].SubItems[0].Text);
                    DomainController.getInstance().AddLobbyMessageToQueue(8, pendingId.ToString());
                    break;
                default:
                    break;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            if (maximized)
            {
                this.Size = minimizedSize;
                this.Location = new Point(this.Location.X, this.Location.Y + maximizedSize.Height - minimizedSize.Height);
                tcSocial.Hide();
                btnAdd.Hide();
                btnRemove.Hide();
                hideButton.Text = "^";
                maximized = false;
            }
            else
            {
                this.Size = maximizedSize;
                this.Location = new Point(this.Location.X, this.Location.Y - maximizedSize.Height + minimizedSize.Height);
                tcSocial.Show();
                btnAdd.Show();
                btnRemove.Show();
                hideButton.Text = "V";
                maximized = true;
            }
        }

        private void sendMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiController.getInstance().createConversation(lvw_Friends.SelectedItems[0].SubItems[0].Text);
        }

        private void cmsFriendChat_Opening(object sender, CancelEventArgs e)
        {
            //don't show if nothing is selected
            if (lvw_Friends.SelectedItems.Count == 0)
            {
                e.Cancel = true;
            }
        }

        private void lvw_Friends_DoubleClick(object sender, EventArgs e)
        {
            //don't do anything if nothing is selected
            if (lvw_Friends.SelectedItems.Count != 0)
            {
                GuiController.getInstance().createConversation(lvw_Friends.SelectedItems[0].SubItems[0].Text);
            }
        }

        private void tcSocial_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tcSocial.SelectedIndex)
            {
                case 0:
                    btnAdd.Text = "Add Friend";
                    btnAdd.Enabled = true;
                    btnRemove.Text = "Remove Friend";
                    btnRemove.Enabled = false;
                    break;
                case 1:
                    btnAdd.Text = "Add Ignore";
                    btnAdd.Enabled = true;
                    btnRemove.Text = "Remove Ignore";
                    btnAdd.Enabled = false;
                    break;
                case 2:
                    btnAdd.Text = "Accept Friend";
                    btnAdd.Enabled = false;
                    btnRemove.Text = "Remove Request";
                    btnRemove.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void lvw_Friends_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableButtons(lvw_Friends.SelectedItems.Count);
        }

        private void lvw_Ignore_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableButtons(lvw_Ignore.SelectedItems.Count);
        }

        private void lvw_Pending_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableButtons(lvw_Pending.SelectedItems.Count);
        }

        #endregion
		 */
        #region Extra Functions

        public void Addfriend(Friend friend)
        {

        }

        public void AddPending(Friend pending)
        {

        }

        public void ClearSocialLists(int choice)
        {

        }

        public void EnableButtons(int count)
        {

        }

        public void RemoveFromLvwPending(string name)
        {

        }

        public void RemoveFromLvwFriends(string name)
        {

        }

        #endregion

    }
}
