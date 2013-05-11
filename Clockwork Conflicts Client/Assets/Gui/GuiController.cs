using UnityEngine;
using System.Collections;

using System.Collections.Generic;

using MMTD_Client.Controls;
using MMTD_Client.Domain;
using MMTD_Client.Network;
using MMTD_Client.Persistence;

namespace MMTD_Client.Gui
{
    public class GuiController
    {
     
        #region Init 

        public const int optimalHeight = 1080;
        public const int optimalWidth = 1920;
        public Vector2 screen { get; set; }
        public Vector2 scale { get; set; }
        public Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

        private static GuiController guiController;
        private PersistenceController persistenceController;
        private DomainController domainController;
        private CreateChannel createChannel;
        private AddUser addUser;
        //public FriendsWindow friendsWindow;
        //public ChannelWindow channelWindow;
        //public ChatWindow broadCastChannel;
        //public GuildWindow guildWindow;
        //public ChatWindow guildChatChannel;
        public List<ChatWindow> channelList = new List<ChatWindow>();

        public string debugText { get; private set; }
        public int maxLines = 100;

        public Queue<string> receiveQueue { get; private set; }
        public Queue<string> debugQueue { get; private set; }

        public int activeChannel { get; set; }

        private GuiController()
        {
            debugQueue = new Queue<string>();
            receiveQueue = new Queue<string>();
            LocalizedStrings.SetLanguage("en");
            DomainController.getInstance().setMode(-1);
            domainController = DomainController.getInstance();
            domainController.SetNetworkController();
            persistenceController = PersistenceController.getInstance();
            persistenceController.getLoginServer();

            screen = new Vector2(Screen.width, Screen.height); //width is x, height is y
            scale = new Vector2(screen.x / optimalWidth, screen.y / optimalHeight);

            SetDebugText("Profit, het programma start!");
            activeChannel = 0;
        }

        public static GuiController getInstance()
        {
            if (guiController == null)
            {
                guiController = new GuiController();
            }
            return guiController;
        }
        #endregion

        #region Control Draw Functions

        public Rect ScaledRect(Rect rect, Rect ParentRect, bool alignRight = false, bool alignBottom = false, bool changePosition = true)
        {
            float x = ParentRect.x + rect.x;
            float y = ParentRect.y + rect.y;
            float width = rect.width * scale.x;
            float height = rect.height * scale.y;

            if (alignRight)
            {
                x = ParentRect.width - (rect.x + rect.width);
            }

            if (alignBottom)
            {
                y = (screenRect.height - (ParentRect.y + ParentRect.height) - rect.y - rect.height);
            }


            if (changePosition)
            {
                x = x * scale.x;
                y = y * scale.y;
            }
            return new Rect(x, y, width, height);
        }

        #endregion

        #region Still Implement

        #region Extra Functions


        public void UserLoggedIn()
        {
            Application.LoadLevel("LoggedInScene");
            /*
            //add extra controls
            friendsWindow = new FriendsWindow();

            broadCastChannel = new ChatWindow(0, "Broadcast Channel", 0);

            channelWindow = new ChannelWindow();

            guildWindow = new GuildWindow();*/
        }

        public void UserLoggedOut()
        {
            Application.LoadLevel("LoginScene");
        }

        public void AddBroadcast(string output, string hexColour)
        {
            ChatChannel broadcastChannel = domainController.GetChannelById(0);
            if (broadcastChannel.reveivedText.Count > maxLines)
            {
                broadcastChannel.reveivedText.Dequeue();
            }
            broadcastChannel.reveivedText.Enqueue(GetColoredText(output, hexColour));
        }

        public void AddGuildChat(string output, string hexColour)
        {
        }

        public void AddConvMessage(string conversationName, string output, string hexColour)
        {

        }

        public ChatWindow AddChatChannel(int channelId, string channelName, int ownerId)
        {
            return null;
        }

        public void ShowChatWindow(ChatWindow window, bool move)
        {

        }

        public ChatWindow createConversation(string name)
        {
            return null;
        }

        public void AddGuildChat()
        {
        }

        public void RemovePendingFriend(string friendName)
        {
        }

        public void RemoveFriend(string friendName)
        {
        }

        public void AddUser(string addType)
        {

        }

        public void ClearSocialLists(int choice)
        {

        }

        #endregion

        public void ShowMessageBox(string text, string caption = "DEBUG")
        {
            SetDebugText("MESSAGEBOX " + caption +": " + text);
        }
        #endregion

        #region Implemented

        public void SendLogin(string username, string password)
        {
            domainController.SendLogin(username.ToLower(), password);
        }

        #endregion


        public void SetDebugText(string text)
        {
            debugText = text;
            if (debugQueue.Count > maxLines)
            {
                debugQueue.Dequeue();
            }
            debugQueue.Enqueue(text);
        }

        public void Update()
        {
            screen = new Vector2(Screen.width, Screen.height);
            screenRect = new Rect(0, 0, Screen.width, Screen.height);
            scale = new Vector2(screen.x / optimalWidth, screen.y / optimalHeight);
        }

        public string GetColoredText(string message, string color)
        {
            return "<color=" + color + "FF>" + message + "</color>";
        }

        public void AddToChat(string text)
        {
            domainController.GetChannelById(activeChannel).reveivedText.Enqueue( "<color=#FFFF00FF>" + text + "</color>");
        }
    }
}
