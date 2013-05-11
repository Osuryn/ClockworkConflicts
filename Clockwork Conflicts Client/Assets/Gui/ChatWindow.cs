using UnityEngine;
using System.Collections;

using System;

//using System.Drawing;
//using System.Windows.Forms;
using MMTD_Client.Gui;
using MMTD_Client.Domain;

namespace MMTD_Client.Gui
{
	//========================================================================
	//  			  CLASS DESCRIPTION : ChatWindow
	//========================================================================
	//- This is the control for a chat window
	//========================================================================
	public class ChatWindow
	{
        #region Init

		//private Size maximizedSize;

		public bool maximized { get; set; }
		//public Size minimizedSize { get; private set; }
		public int channelId { get; set; }

		public int ownerId { get; set; }

		public string channelName { get; set; }

		private Rect windowRect = new Rect (200, 200, 300, 200);
		private GuiController guiController;

		public ChatWindow (int channelId, string channelName, int ownerId)
		{		
			guiController = GuiController.getInstance ();
			guiController.SetDebugText("Het chatwindow is aangemaakt");
			this.channelId = channelId;
            this.channelName = channelName;
            this.ownerId = ownerId;
		}

        #endregion
		/*
        #region Event Handlers

        private void hideButton_Click(object sender, EventArgs e)
        {
            if (maximized)
            {
                this.Size = minimizedSize;
                this.Location = new Point(this.Location.X, this.Location.Y + maximizedSize.Height - minimizedSize.Height);
                output.Hide();
                input.Hide();
                hideButton.Text = "^";
                maximized = false;
            }
            else
            {
                this.Size = maximizedSize;
                this.Location = new Point(this.Location.X, this.Location.Y - maximizedSize.Height + minimizedSize.Height);
                output.Show();
                input.Show();
                hideButton.Text = "V";
                maximized = true;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && input.Text != "")
            {
                if (this.channelId == -1)
                {
                    DomainController.getInstance().AddChatMessageToQueue(5, DomainController.getInstance().myGuild.guildId, input.Text);
                }
                else if (this.channelId == 0)
                {
                    DomainController.getInstance().AddChatMessageToQueue(1, 0, input.Text);
                }
                else if (this.channelId > 100)
                {
                    DomainController.getInstance().AddChatMessageToQueue(4, this.channelId - 100, input.Text);
                }
                else
                {
                    DomainController.getInstance().AddChatMessageToQueue(3, channelId, input.Text);
                }
                input.Text = "";
            }
        }
        
		private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            dragable = true;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            dragable = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragable)
            {
                this.Location = new Point(MousePosition.X - GuiController.getInstance().Location.X - 5, MousePosition.Y - GuiController.getInstance().Location.Y - 30);
            }
        }
        
        #endregion
		 */
        #region Extra Functions

		public void AddText (string text, string hexColour)
		{
		}

        #endregion	
		
		// Update is called once per frame
		void Update ()
		{
			
		}
	}
}
