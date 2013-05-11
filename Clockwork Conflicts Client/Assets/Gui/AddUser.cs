using System;
using System.Threading;

//using System.Drawing;
//using System.Windows.Forms;

using MMTD_Client.Domain;

namespace MMTD_Client.Gui
{
    public partial class AddUser
    {

        #region Init

        public bool destroyed;

        public AddUser()
        {
        }

        public AddUser(string addType)
        {
        }

        #endregion
		/*
        #region Event Handlers

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text != "")
            {
                if (lbl_Title.Text == "Add Friend")
                {
                    DomainController.getInstance().AddLobbyMessageToQueue(6, txtName.Text);
                    destroyed = true;
                    this.Dispose();
                }
                else if (lbl_Title.Text == "Add Ignore")
                {
                    DomainController.getInstance().AddLobbyMessageToQueue(9, txtName.Text);
                    destroyed = true;
                    this.Dispose();
                }
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            destroyed = true;
            this.Dispose();
        }

        #endregion
		 */
        #region Extra Functions

        public void Flash(int times, int delay)
        {
        }

        #endregion

    }
}
