using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    class MessageBox : Window
    {
        public static int Count;

        public delegate void Function();

        public int messageBoxId { get; set; }
        public bool answered { get; set; }
        public Action yesAction;
        public Action noAction;

        private Label lbl_Text = new Label("lbl_Text" + windowCount);
        private Button btn_Yes = new Button("btn_Yes" + windowCount);
        private Button btn_No = new Button("btn_No" + windowCount);

        public MessageBox(string name) : base(name)
        {
            messageBoxId = Count;
            Count++;
            this.SetRect(new Rect(810, 390, 300, 200));
            this.windowFunction = WindowFunction;
            this.fontSize = 20;

            lbl_Text.location = new Point(20, 25);
            lbl_Text.fontSize = 16;

            btn_Yes.text = LocalizedStrings.str_yes;
            btn_Yes.location = new Point(20, 150);
            btn_Yes.size = new Size(50, 0);
            btn_Yes.fontSize = 16;
            btn_Yes.Clicked += new EventHandler(Yes_button_Pressed);

            btn_No.text = LocalizedStrings.str_no;
            btn_No.location = new Point(230, 150);
            btn_No.size = new Size(50, 0);
            btn_No.fontSize = 16;
            btn_No.Clicked += new EventHandler(No_button_Pressed);

            children.Add(lbl_Text);
            children.Add(btn_Yes);
            children.Add(btn_No);
        }

        void WindowFunction(int windowID)
        {
            GUI.DragWindow(guiController.ScaledRect(new Rect(0, 0, 10000, 40), guiController.screenRect));
            this.RenderChildren();
        }

        private void Yes_button_Pressed(object Sender, EventArgs e)
        {
            yesAction();
            answered = true;
        }

        private void No_button_Pressed(object Sender, EventArgs e)
        {
            noAction();
            answered = true;
        }

        public void SetMessage(string text)
        {
            lbl_Text.text = text;
        }
    }
}
