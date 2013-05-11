using UnityEngine;

using System;
using System.Threading;
using System.Collections;

using MMTD_Client.Gui;
using MMTD_Client.Controls;

public class LoginGui : MonoBehaviour
{
    public GUISkin skin;

    //Controls
    private Box box_Login = new Box("box_Login");
    private Label lbl_Username = new Label("lbl_Username");
    private Label lbl_Password = new Label("lbl_Password");
    private TextField txf_Username = new TextField("txf_Username");
    private TextField txf_Password = new TextField("txf_Password");
    private Button btn_Login = new Button("btn_Login");

    private GuiController guiController;

    // Use this for initialization
    void Start()
    {
        guiController = GuiController.getInstance();
        InitGUI();
    }

    void InitGUI()
    {
        Rect loginBox = new Rect(1100, 200, 730, 430);

        box_Login.text = LocalizedStrings.str_login;
        box_Login.SetRect(loginBox);
        box_Login.fontSize = 25;

        lbl_Username.text = LocalizedStrings.str_username;
        lbl_Username.location = new Point(50, 70);
        lbl_Username.parentSurface = loginBox;
        lbl_Username.fontSize = 25;

        lbl_Password.text = LocalizedStrings.str_password;
        lbl_Password.location = new Point(50, 230);
        lbl_Password.parentSurface = loginBox;
        lbl_Password.fontSize = 25;

        txf_Username.location = new Point(250, 60);
        txf_Username.size = new Size(370, 0);
        txf_Username.parentSurface = loginBox;
        txf_Username.fontSize = 25;

        txf_Password.location = new Point(250, 220);
        txf_Password.size = new Size(370, 0);
        txf_Password.parentSurface = loginBox;
        txf_Password.fontSize = 25;
        txf_Password.passwordField = true;

        btn_Login.text = LocalizedStrings.str_login;
        btn_Login.location = new Point(350, 350);
        btn_Login.size = new Size(200, 0);
        btn_Login.parentSurface = loginBox;
        btn_Login.fontSize = 25;
        btn_Login.Clicked += new EventHandler(Login_button_Pressed);
    }

    private void OnGUI()
    {
        GUI.skin = skin;

        box_Login.Render();
        lbl_Username.Render();
        lbl_Password.Render();
        txf_Username.Render();
        txf_Password.Render();
        btn_Login.Render();

        //username = guiController.DrawTextField ("txt_Username", username, 250, 60, 370, loginBox, 25);              
        //password = guiController.DrawTextField ("txt_Password", password, 250, 220, 370, loginBox, 25);



        Event e = Event.current;

        if (e.keyCode == KeyCode.Return)
        {
            switch (GUI.GetNameOfFocusedControl())
            {
                case "txf_Password":
                    Login_button_Pressed(this, EventArgs.Empty);
                    break;
                case "txf_Username":
                    GUI.FocusControl("txf_Password");
                    break;
                default:
                    //Debug.Log("Pressed enter");
                    break;
            }
        }
    }

    #region Event Handlers
    private void Login_button_Pressed(object Sender, EventArgs e)
    {
        if (txf_Username.text != "" && txf_Password.text != "")
        {
            guiController.SendLogin(txf_Username.text, txf_Password.text);
            txf_Password.text = "";
        }
        else
        {
            guiController.SetDebugText("Please fill in both username and password, incomplete credentials");
        }
    }

    #endregion
}
