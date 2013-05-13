using UnityEngine;

using System;
using System.Collections.Generic;
using System.Timers;

using MMTD_Client.Controls;
using MMTD_Client.Domain;
using MMTD_Client.Gui;

public class ControllerObjectScript : MonoBehaviour
{

    #region Init

    public Texture2D loginBackground;

    public GuiController guiController { get; private set; }
    public DomainController domainController { get; private set; }

    public List<Control> controls { get; set; }

    private bool logginOver = false;

    //FPSCounter
    public float updateInterval = 0.5F;
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    #region Controls

    //General
    private Label lbl_Debug = new Label("lbl_Debug");
    private Window wnd_Console = new Window("wnd_Console");
    private TextField txf_Console = new TextField("txf_Console");
    private TextArea txa_Console = new TextArea("txa_Console");
    private ScrollView scv_Console = new ScrollView("scv_Console");
    private Label lbl_FPSCounter = new Label("lbl_FPSCounter");

    //Login
    private Box box_Login = new Box("box_Login");
    private Label lbl_Username = new Label("lbl_Username");
    private Label lbl_Password = new Label("lbl_Password");
    private TextField txf_Username = new TextField("txf_Username");
    private TextField txf_Password = new TextField("txf_Password");
    private Button btn_Login = new Button("btn_Login");

    //Logged In
    private TextField txf_Send = new TextField("txf_Send");
    private Button btn_Send = new Button("btn_Send");
    private Window wnd_Chat = new Window("wnd_Chat");
    private Window wnd_Social = new Window("wnd_Social");
    private Window wnd_Guild = new Window("wnd_Guild");
    private Window wnd_CreateChannel = new Window("wnd_CreateChannel");
    private TextArea txa_Received = new TextArea("txa_Received");
    private ScrollView scv_Chat = new ScrollView("scv_Chat");
    private Toolbar tlb_Channels = new Toolbar("tlb_Channels");
    public TextField txf_ChannelName = new TextField("txf_ChannelName");
    private Button btn_CreateChannel = new Button("btn_CreateChannel");
    private Toolbar tlb_Menu = new Toolbar("tlb_Menu");

    #endregion

    // Use this for initialization
    void Start()
    {
        timeleft = updateInterval;
        controls = new List<Control>();
        guiController = GuiController.getInstance();
        guiController.SetControllerObject(this);
        domainController = DomainController.getInstance();
        domainController.SetGuiController();
        DontDestroyOnLoad(transform.gameObject);
        wnd_Console.visible = false;
        InitGeneralGUI();
        InitLoginGUI();
        InitLoggedInGUI();
    }

    #region Init GUI

    private void InitGeneralGUI()
    {
        lbl_Debug.text = guiController.debugText;
        lbl_Debug.location = new Point(50, 1000);
        lbl_Debug.fontSize = 18;

        txf_Console.location = new Point(20, 320);
        txf_Console.size = new Size(530, 0);
        txf_Console.fontSize = 18;

        txa_Console.SetRect(new Rect(0, 0, 530, 0));
        txa_Console.fontSize = 18;

        scv_Console.SetRect(new Rect(20, 30, 560, 290));
        scv_Console.totalArea = new Rect();
        scv_Console.autoScroll = true;

        wnd_Console.text = LocalizedStrings.str_console;
        wnd_Console.SetRect(new Rect(660, 340, 600, 400));
        wnd_Console.windowFunction = WindowFunction;
        wnd_Console.fontSize = 20;

        scv_Console.children.Add(txa_Console);

        wnd_Console.children.Add(scv_Console);
        wnd_Console.children.Add(txf_Console);

        lbl_FPSCounter.location = new Point(20, 35);

        controls.Add(lbl_Debug);
        controls.Add(wnd_Console);
        controls.Add(lbl_FPSCounter);
    }

    void InitLoginGUI()
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

        box_Login.AddChild(lbl_Username);
        box_Login.AddChild(lbl_Password);
        box_Login.AddChild(txf_Username);
        box_Login.AddChild(txf_Password);
        box_Login.AddChild(btn_Login);

        controls.Add(box_Login);
    }

    private void InitLoggedInGUI()
    {
        txf_Send.location = new Point(20, 320);
        txf_Send.size = new Size(430, 0);
        txf_Send.fontSize = 18;

        btn_Send.text = LocalizedStrings.str_send;
        btn_Send.location = new Point(480, 320);
        btn_Send.size = new Size(75, 0);
        btn_Send.fontSize = 20;
        btn_Send.Clicked += new EventHandler(Send_button_Pressed);

        wnd_Chat.text = LocalizedStrings.str_broadcast;
        wnd_Chat.SetRect(new Rect(50, 600, 600, 400));
        wnd_Chat.windowFunction = WindowFunction;
        wnd_Chat.fontSize = 20;
        wnd_Chat.visible = false;

        wnd_Social.text = LocalizedStrings.str_social;
        wnd_Social.SetRect(new Rect(1550, 40, 300, 600));
        wnd_Social.windowFunction = WindowFunction;
        wnd_Social.fontSize = 25;
        wnd_Social.visible = false;

        wnd_Guild.text = LocalizedStrings.str_guild;
        wnd_Guild.SetRect(new Rect(20, 40, 500, 200));
        wnd_Guild.windowFunction = WindowFunction;
        wnd_Guild.fontSize = 25;
        wnd_Guild.visible = false;

        wnd_CreateChannel.text = LocalizedStrings.str_createChannel;
        wnd_CreateChannel.SetRect(new Rect(50, 480, 450, 100));
        wnd_CreateChannel.windowFunction = WindowFunction;
        wnd_CreateChannel.fontSize = 18;
        wnd_CreateChannel.visible = false;

        txa_Received.SetRect(new Rect(0, 0, 530, 0));
        txa_Received.fontSize = 18;

        scv_Chat.SetRect(new Rect(20, 80, 560, 200));
        scv_Chat.totalArea = new Rect();
        scv_Chat.autoScroll = true;

        tlb_Channels.SetRect(new Rect(15, 40, 530, 40));
        tlb_Channels.selectedIndex = 0;

        txf_ChannelName.location = new Point(20, 40);
        txf_ChannelName.size = new Size(300, 0);
        txf_ChannelName.fontSize = 18;

        btn_CreateChannel.text = LocalizedStrings.str_create;
        btn_CreateChannel.location = new Point(330, 42);
        btn_CreateChannel.size = new Size(75, 0);
        btn_CreateChannel.fontSize = 15;
        btn_CreateChannel.Clicked += new EventHandler(CreateChannel_button_Pressed);

        tlb_Menu.SetRect(new Rect(5, 5, 1915, 30));
        tlb_Menu.visible = false;

        scv_Chat.children.Add(txa_Received);

        wnd_Chat.children.Add(tlb_Channels);
        wnd_Chat.children.Add(scv_Chat);
        wnd_Chat.children.Add(txf_Send);
        wnd_Chat.children.Add(btn_Send);

        wnd_CreateChannel.children.Add(txf_ChannelName);
        wnd_CreateChannel.children.Add(btn_CreateChannel);

        tlb_Menu.items.Add("Chat");
        tlb_Menu.items.Add("Social");
        tlb_Menu.items.Add("Guild");
        tlb_Menu.items.Add("Play");

        controls.Add(tlb_Menu);
        controls.Add(wnd_CreateChannel);
        controls.Add(wnd_Guild);
        controls.Add(wnd_Social);
        controls.Add(wnd_Chat);
    }

    #endregion

    #endregion

    // Update is called once per frame
    void Update()
    {
        UpdateFPSCounter();
        List<MessageBox> toDelete = new List<MessageBox>();
        foreach (Control control in controls)
        {
            if (control is MessageBox)
            {
                MessageBox delete = (MessageBox)control;
                if (delete.answered)
                {
                    toDelete.Add((MessageBox)control);
                }
            }
        }
        foreach (MessageBox messageBox in toDelete)
        {
            controls.Remove(messageBox);
        }
        guiController.Update();
    }

    public void UpdateFPSCounter()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            lbl_FPSCounter.text = format;

            if (fps < 30)
                lbl_FPSCounter.color = Color.yellow;
            else
                if (fps < 10)
                    lbl_FPSCounter.color = Color.red;
                else
                    lbl_FPSCounter.color = Color.green;
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    void OnGUI()
    {
        lbl_Debug.text = guiController.debugText;
        if (!domainController.loggedIn)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), loginBackground);
        }

        if (guiController.fpsCounter)
        {
            lbl_FPSCounter.visible = true;
        }
        else
        {
            lbl_FPSCounter.visible = false;
        }

        foreach (Control control in controls)
        {
            if (control.visible)
            {
                control.Render();
            }
        }

        #region General

        txa_Console.lines = guiController.debugQueue;
        //lbl_Debug.Render();
        Event e = Event.current;


        if (e.keyCode == KeyCode.F12)
        {
            if (wnd_Console.EventCompleted("F12"))
            {
                if (wnd_Console.visible)
                {
                    wnd_Console.visible = false;
                }
                else
                {
                    wnd_Console.visible = true;
                }
            }
        }

        if (e.keyCode == KeyCode.Return)
        {
            switch (GUI.GetNameOfFocusedControl())
            {
                case "txf_Console":
                    ExecuteCommand();
                    break;
                default:
                    //Debug.Log("Pressed enter");
                    break;
            }
        }

        //wnd_Console.Render();
        #endregion

        #region Login

        #endregion

        #region LoggedIn

        if (domainController.loggedIn && !logginOver)
        {
            tlb_Menu.visible = true;
            wnd_Chat.visible = true;
            logginOver = true;
            box_Login.visible = false;
        }

        //tlb_Menu.Render();

        wnd_Chat.text = domainController.GetChannelById(guiController.activeChannel).channelName;
        //wnd_Chat.Render();
        //wnd_Social.Render();
        //wnd_Guild.Render();
        //wnd_CreateChannel.Render();

        switch (tlb_Menu.selectedIndex)
        {
            case 0: //Chat
                if (wnd_Chat.visible)
                {
                    wnd_Chat.visible = false;
                }
                else
                {
                    wnd_Chat.visible = true;
                }
                tlb_Menu.selectedIndex = -1;
                break;
            case 1: //Social
                if (wnd_Social.visible)
                {
                    wnd_Social.visible = false;
                }
                else
                {
                    wnd_Social.visible = true;
                }
                tlb_Menu.selectedIndex = -1;
                break;
            case 2: //Guild
                if (wnd_Guild.visible)
                {
                    wnd_Guild.visible = false;
                }
                else
                {
                    wnd_Guild.visible = true;
                }
                tlb_Menu.selectedIndex = -1;
                break;
            case 3: //Play
                tlb_Menu.selectedIndex = -1;
                break;
            default:
                break;
        }

        Event e2 = Event.current;

        if (GUI.GetNameOfFocusedControl() == "txf_Send")
        {
            switch (e2.keyCode)
            {
                case KeyCode.Return:
                    Send_button_Pressed(this, EventArgs.Empty);
                    break;
                case KeyCode.UpArrow:
                    if (txf_Send.EventCompleted("UpArrow"))
                    {
                        if (domainController.chatHistoryPosition > 0)
                        {
                            domainController.chatHistoryPosition--;
                            txf_Send.text = domainController.chatHistory[domainController.chatHistoryPosition];
                        }
                    }
                    break;
                case KeyCode.DownArrow:
                    if (txf_Send.EventCompleted("DownArrow"))
                    {
                        if (domainController.chatHistoryPosition < domainController.chatHistory.Count - 1)
                        {
                            domainController.chatHistoryPosition++;
                            txf_Send.text = domainController.chatHistory[domainController.chatHistoryPosition];
                        }
                    }
                    break;
                default:
                    //Debug.Log("Pressed enter");
                    break;
            }
        }

        Event e3 = Event.current;

        if (e3.keyCode == KeyCode.Return)
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

        if (domainController.guildInfoReceived)
        {
            wnd_Guild.text = domainController.myGuild.guildName;
        }

        #endregion
    }

    void ExecuteCommand()
    {
        if (txf_Console.text != "")
        {
            if (txf_Console.text[0] == '/')
            {
                domainController.Command(txf_Console.text.Substring(1), false);
            }
            else
            {
                guiController.SetDebugText("Commands start with /");
            }
        }
        txf_Console.text = "";
    }

    void OnApplicationQuit()
    {
        if (domainController.loggedIn)
        {
            domainController.SendLogout();
        }
    }

    void WindowFunction(int windowID)
    {

        GUI.DragWindow(guiController.ScaledRect(new Rect(0, 0, 10000, 40), guiController.screenRect));
        if (windowID == wnd_Console.id)
        {
            //txa_Console.text = guiController.debugLog;
            wnd_Console.RenderChildren();
        }
        else if (windowID == wnd_Chat.id)
        {
            tlb_Channels.items.Clear();
            foreach (ChatChannel channel in domainController.channelList)
            {
                if (channel.joined)
                {
                    tlb_Channels.items.Add(channel.channelName);
                }
            }
            tlb_Channels.items.Add(LocalizedStrings.str_createChannel);
            if (tlb_Channels.selectedIndex != tlb_Channels.items.Count - 1)
            {
                guiController.activeChannel = domainController.GetChannelByName(tlb_Channels.items[tlb_Channels.selectedIndex]).channelId;
            }
            else
            {
                if (wnd_CreateChannel.visible)
                {
                    wnd_CreateChannel.visible = false;
                }
                else
                {
                    wnd_CreateChannel.visible = true;
                }
            }
            txa_Received.lines = domainController.GetChannelById(guiController.activeChannel).reveivedText;
            wnd_Chat.RenderChildren();

        }
        else if (windowID == wnd_Social.id)
        {
            string status = "";
            GUILayout.Label("Friends: ");
            foreach (Friend friend in domainController.friendList)
            {
                if (friend.isOnline)
                {
                    status = "Online";
                }
                else
                {
                    status = "Offline";
                }
                GUILayout.Label(friend.friendName + "\t\t\t\t\t" + status);
            }
            GUILayout.Label("\nPending friends: ");
            foreach (Friend friend in domainController.pendingList)
            {
                if (friend.isOnline)
                {
                    status = "Online";
                }
                else
                {
                    status = "Offline";
                }
                GUILayout.Label(friend.friendName + "\t\t\t\t\t" + status);
            }
            GUILayout.Label("\n\n\n\nChannels: ");
            foreach (ChatChannel channel in DomainController.getInstance().channelList)
            {
                if (channel.userList != null)
                {
                    GUILayout.Label(channel.channelName + "\t\t\t\t\t" + channel.userList.Count);
                }
                else
                {
                    GUILayout.Label(channel.channelName + "\t\t\t\t\t Users unknown");
                }
            }
            GUILayout.Label("\n\n\n\nParty: ");
            if (domainController.myParty != null)
            {
                GUILayout.Label("id: " + domainController.myParty.partyId);
                foreach (Account partyMember in domainController.myParty.userList)
                {
                    GUILayout.Label(partyMember.screenName);
                }
            }
            else
            {
                GUILayout.Label("You are not in a party!");
            }
        }
        else if (windowID == wnd_Guild.id)
        {
            foreach (GuildMember member in domainController.myGuild.userList)
            {
                GUILayout.Label(member.screenName + "\t\t\t\t\t" + member.guildFlags);
            }
        }
        else if (windowID == wnd_CreateChannel.id)
        {
            wnd_CreateChannel.RenderChildren();
        }
    }

    public void ShowMessageBox(string name, string text, Action yesAction, Action NoAction)
    {
        MessageBox box = new MessageBox(name);
        box.text = name;
        box.SetMessage(text);
        box.yesAction = yesAction;
        box.noAction = NoAction;
        controls.Add(box);
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

    private void Send_button_Pressed(object Sender, EventArgs e)
    {
        if (txf_Send.text != "")
        {
            if (txf_Send.text[0] != '/')
            {
                domainController.AddChatMessageToQueue(3, guiController.activeChannel, txf_Send.text);
            }
            else
            {
                domainController.Command(txf_Send.text.Substring(1), true);
            }
            domainController.chatHistory.Add(txf_Send.text);
            domainController.chatHistoryPosition = domainController.chatHistory.Count;
        }
        txf_Send.text = "";
    }

    private void CreateChannel_button_Pressed(object Sender, EventArgs e)
    {
        domainController.AddLobbyMessageToQueue(1, txf_ChannelName.text);
        txf_ChannelName.visible = false;
        txf_ChannelName.text = "";
    }

    #endregion
}