using System;
using System.Threading;

//using System.Windows.Forms;

using MMTD_Client.Domain;

namespace MMTD_Client.Gui
{
    public partial class CreateChannel
    {
        
        #region Init

        public bool destroyed;

        public CreateChannel()
        {
        }

        #endregion
		/*
        #region Event Handlers

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            destroyed = true;
            this.Dispose();
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            if (this.txt_Input.Text != "")
            {
                destroyed = true;
                DomainController.getInstance().AddLobbyMessageToQueue(1, txt_Input.Text);
                this.Dispose();   
            }
        }

        private void txt_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && this.txt_Input.Text != "")
            {
                destroyed = true;
                DomainController.getInstance().AddLobbyMessageToQueue(1, txt_Input.Text);
                this.Dispose();   
            }
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
