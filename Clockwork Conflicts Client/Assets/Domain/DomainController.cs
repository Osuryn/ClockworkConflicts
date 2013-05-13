using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using MMTD_Client.Gui;
using MMTD_Client.Network;
using MMTD_Client.Persistence;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Sockets;


namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : DomainController
    //========================================================================
    //- This class controls most of the functionality in the program
    //========================================================================
    public class DomainController
    {

        #region Init

        enum MessageType { ServerOnly, Broadcast, Party, Room, Conversation };

        private static DomainController domainController;
        private PersistenceController persistenceController;
        private NetworkController networkController;
        private GuiController guiController;

        public Account myAccount { get; set; }
        public Guild myGuild { get; set; }
        public Party myParty { get; set; }
        public Server loginServer { get; private set; }
		
		public Queue<string> IncommingLobbyMessage{ get; set; }
		public Queue<string> IncommingChatMessage{ get; set; }
        private List<Message> chatMessageQueue = new List<Message>();
        private List<Message> lobbyMessageQueue = new List<Message>();
        public List<Friend> friendList { get; set; }
        public List<Friend> pendingList { get; set; }
        private List<int> onlineIdList { get; set; }

        public List<Account> FullyKnownAccounts { get; set; }

        public List<string> chatHistory = new List<string>();
        public int chatHistoryPosition;

        public List<ChatChannel> channelList = new List<ChatChannel>();
		
        public Thread LobbyMessageHandler { get; set; }
        public Thread ChatMessageHandler { get; set; }

        public bool guildInfoReceived = false;
        public bool loggedIn { get; set; }
		private bool ClientOn = true;

        public int invitedParty { get; set; }
        public int invitingPlayer { get; set; }

        private bool waitForAccount = false;

        private DomainController()
        {
            loggedIn = false;
            friendList = new List<Friend>();
            pendingList = new List<Friend>();
			this.IncommingChatMessage = new Queue<string>();
			this.IncommingLobbyMessage = new Queue<string>();
            myGuild = null;
            persistenceController = PersistenceController.getInstance();
            channelList.Add(new ChatChannel(true, 0, "Broadcast"));
			this.startThreads();
        }

        public void SetGuiController()
        {
            guiController = GuiController.getInstance();
        }

        public static DomainController getInstance()
        {
            if (domainController == null)
            {
                domainController = new DomainController();
            }
            return domainController;
        }

        public void SetNetworkController()
        {
            networkController = NetworkController.getInstance();
        }

        #endregion

        #region Accounts

        public Account getAccountByID(int id)
        {
            foreach (Account account in FullyKnownAccounts)
            {
                if (account.accountId == id)
                    return account;
            }
            AddLobbyMessageToQueue(11, id.ToString());
            waitForAccount = true;
            while (!waitForAccount)
            {
                Thread.Sleep(50);
            }
            return null;
        }

        #endregion

        #region Messages

        #region Incomming Message

        private void HandleIncomingChatMessage(string output)
        {

            guiController.SetDebugText("RECEIVED CHAT: " + output);

            string backup;
            string hexColour = "#FFFFFF";
            int index;

            //split up string
            index = output.IndexOf("|");
            backup = output.Substring(1, index - 1);
            output = output.Substring(index + 1);

            //get type
            index = backup.IndexOf("]");
            int type = Convert.ToInt32(backup.Substring(0, index));
            backup = backup.Substring(index + 1);

            //get colour
            index = backup.IndexOf("<");
            if (index != -1)
            {
                hexColour = backup.Substring(index + 1, 7);
                backup = backup.Substring(index + 8);
            }

            switch (type)
            {
                //broadcast
                case 1:
                    guiController.AddBroadcast(output, hexColour);
                    break;
                //channel
                case 3:
                    AddChannelMessage(output, hexColour);
                    break;
                //conversation
                case 4:
                    //string manipulate on output to extract sender name
                    index = output.IndexOf(":");
                    string conversationName = output.Substring(0, index);
                    output = output.Substring(index + 1);

                    guiController.AddConvMessage(conversationName, output, hexColour);
                    break;
                //Guild
                case 5:
                    guiController.AddGuildChat(output, hexColour);
                    break;
                default:
                    guiController.AddBroadcast(output, hexColour);
                    break;
            }
        }

        private void HandleIncomingLobbyMessage(string output)
        {
            lobbyMessageQueue.Add(new LobbyMessage(255));
            //[1,8]3 [channel created, user Dakos]channelid 3  
            string data = output.Substring(1);
            int index = data.IndexOf(",");
            int type = Convert.ToInt32(data.Substring(0, index));
            index = -1;
            index = data.IndexOf("]");
            data = data.Substring(index + 1);

            guiController.SetDebugText("RECEIVED LOBBY: " + output);

            switch (type)
            {
                //Put server message on chat (yellow)
                case 0:
                    guiController.AddToChat(data);
                    break;
                case 1:
                    AddChatChannel(data);
                    guiController.SetDebugText("Added Channel");
                    break;
                case 2:
                    FormatFriendList(data);
                    guiController.SetDebugText("Formatted friendlist");
                    break;
                case 3:
                    FillFriendList(data);
                    guiController.SetDebugText("Filled friendList");
                    break;
                case 4:
                    FriendOnline(Convert.ToInt32(data));
                    guiController.SetDebugText("Friend came online");
                    break;
                case 5:
                    FriendOffline(Convert.ToInt32(data));
                    guiController.SetDebugText("Friend went offline");
                    break;
                // SEND FRIENDREQUEST
                case 6:
                    index = data.IndexOf(",");
                    int accepted = Convert.ToInt32(data.Substring(0, index));
                    data = data.Substring(index + 1);
                    string FriendId = data;

                    if (accepted == 1)
                    {
                        guiController.ShowMessageBox("Friend request to " + data + " successfully sent");
                    }
                    else if (accepted == 0)
                    {
                        guiController.ShowMessageBox("Failed to send friend request, common causes:\n-Name does not exist\n-you already have said user as friend");
                    }
                    guiController.SetDebugText("Friendrequest response");
                    break;
                // ACCEPTED A PENDING FRIEND (completed/failed?)
                case 7:
                    AcceptedPendingFriend(data);
                    guiController.SetDebugText("Accepted pending friend");
                    break;
                // REJECTED FRIENDSHIP REQUEST (completed/failed?)
                case 8:
                    RejectedPendingFriend(data);
                    guiController.SetDebugText("rejected pending friend");
                    break;
                // REMOVED A FRIEND (completed/failed?)
                case 9:
                    RemovedFriendship(data);
                    guiController.SetDebugText("Friend Removed");
                    break;
                case 10:
                    FillPendingList(data);
                    guiController.SetDebugText("Pending friendList");
                    break;
                //get Account data
                case 11:
                    ReceiveAccountInfo(data, false);
                    waitForAccount = false;
                    break;
                case 20:
                    GetChatchannels(data);
                    guiController.SetDebugText("Got all chatchannels");
                    break;
                    //join channel and get members
                case 21:
                    JoinChannel(data);
                    break;
                case 32: //get guildInfo
                    GetGuildInfo(data);
                    guiController.SetDebugText("Got guild info");
                    break;
                case 35: //get Guildmembers
                    GetGuildMembers(data);
                    guiController.SetDebugText("Got guildmembers");
                    break;
                //party invite sent
                case 40:
                    myAccount.partyID = Convert.ToInt32(data);
                    myParty = new Party(myAccount, myAccount.partyID);
                    guiController.AddToChat("Party invite sent.");
                    break;
                //get party invite
                case 41:
                    GetPartyInvite(data);
                    break;
                //new party member
                case 42:
                    guiController.AddToChat(data + " has joined the party!");
                    break;
                //your party invitation was declined
                case 43:
                    guiController.AddToChat(data + " has rejected your party request.");
                    break;
                //get party members
                case 44:
                    FormatPartyMembers(data);
                    break;
                default:
                    //#if DEBUG
                    guiController.ShowMessageBox("Received unknown message type from lobbyserver: " + type);
                    //#endif
                    break;
            }

        }

        #endregion

        public void AddChatMessageToQueue(int type, int receiver, string message)
        {
            chatMessageQueue.Add(new ChatMessage(DateTime.Now.ToString(), myAccount.accountId, receiver, type, message));
        }

        public void AddLobbyMessageToQueue(int type, string message = "")
        {
            lobbyMessageQueue.Add(new LobbyMessage(type, myAccount.accountId, message));
        }

        public Message GetNextMessageFromQueue(string sender)
        {
            List<Message> messageQueue = null;
            Message nextMessage = null;
            switch (sender)
            {
                case "ChatSender":
                    messageQueue = chatMessageQueue;
                    break;
                case "LobbySender":
                    messageQueue = lobbyMessageQueue;
                    break;
                default:
#if DEBUG
                    guiController.ShowMessageBox("Trying to access MessageQueue from unknown sender, it either doesn't exist or has no MessageQueue: " + sender);
#endif
                    break;
            }
            if (messageQueue != null)
            {
                if (messageQueue.Count > 0)
                {
                    try
                    {
                        nextMessage = messageQueue.First();
                        messageQueue.RemoveAt(0);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else
            {
#if DEBUG
                guiController.ShowMessageBox("Something went wrong with asigning the MessageQueue for: " + sender);
#endif
            }

            return nextMessage;
        }

        public void GetMessage(string listener, string output)
        {
            switch (listener.ToLower())
            {
                case "chatlistener":
                    HandleIncomingChatMessage(output);
                    break;
                case "lobbylistener":
                    HandleIncomingLobbyMessage(output);
                    break;
                default:
#if DEBUG
                    guiController.ShowMessageBox("Got message from unkown listener: " + listener);
#endif
                    break;
            }

        }

        #endregion

        #region Friends

        private void FriendOnline(int friendId)
        {
            foreach (Friend friend in friendList)
            {
                if (friend.friendId == friendId)
                {
                    friend.isOnline = true;
                }
            }
        }

        private void FriendOffline(int friendId)
        {
            foreach (Friend friend in friendList)
            {
                if (friend.friendId == friendId)
                {
                    friend.isOnline = false;
                }
            }
        }

        public bool IsOnlineFriend(int id)
        {
            foreach (int friend in onlineIdList)
            {
                if (friend == id)
                {
                    return true;
                }
            }
            return false;
        }

        private void FillFriendList(string data)
        {
            int index = -1;
            string dataBackup = data;
            index = data.IndexOf("|");
            while (index != -1)
            {
                try
                {
                    string localBackup = dataBackup.Substring(0, index);

                    index = data.IndexOf(",");
                    if (index != -1)
                    {
                        int id = Convert.ToInt32(localBackup.Substring(0, index));

                        localBackup = localBackup.Substring(index + 1);
                        index = localBackup.IndexOf(",");
                        string name = localBackup.Substring(0, index);
                        sbyte flags = Convert.ToSByte(Convert.ToInt16(localBackup.Substring(index + 1)));
                        bool online = IsOnlineFriend(id);

                        index = -1;
                        index = dataBackup.IndexOf("|");
                        dataBackup = dataBackup.Substring(index + 1);
                        try
                        {
                            friendList.Add(new Friend(id, name, false, flags, online));
                            //guiController.SetDebugText("Friend " + name + " added!");
                        }
                        catch (Exception ex)
                        {
                            guiController.SetDebugText("Problem adding friend " + id + "/" + name + "/" + flags + "/" + online + " to FriendList: " + ex.ToString());
                        }
                    }


                    index = -1;
                    index = dataBackup.IndexOf("|");
                }
                catch (Exception e)
                {
                    guiController.SetDebugText("General problem adding friend: " + e.ToString());
                }
            }

            if (dataBackup.Length != 0)
            {
                string localBackup = dataBackup;
                index = -1;
                index = data.IndexOf(",");
                if (index > -1)
                {
                    int id = Convert.ToInt32(localBackup.Substring(0, index));

                    localBackup = localBackup.Substring(index + 1);
                    index = localBackup.IndexOf(",");
                    string name = localBackup.Substring(0, index);
                    sbyte flags = Convert.ToSByte(Convert.ToInt16(localBackup.Substring(index + 1)));
                    bool online = IsOnlineFriend(id);

                    friendList.Add(new Friend(id, name, false, flags, online));
                }
            }
        }

        public void FormatFriendList(string data)
        {
            //MessageBox.Show("Received friendlist from lobbyserver: " + data);
            int index = -1;
            index = data.IndexOf("|");
            string friendIds = data.Substring(0, index);
            string onlineIds = data.Substring(index + 1);
            List<int> friends = getIntListFromData(friendIds);
            onlineIdList = getIntListFromData(onlineIds);

            List<int> removeList = new List<int>();
            int counter;
            string output;
            while (friends.Count > 19)
            {
                counter = 0;
                output = "";
                foreach (int friendId in friends)
                {
                    if (counter <= 18)
                    {
                        output += friendId + ",";
                        removeList.Add(friendId);
                        counter++;
                    }
                    else if (counter == 19)
                    {
                        output += friendId + "";
                        removeList.Add(friendId);
                        counter++;
                    }
                }

                //SEND MESSAGE
                LobbyMessage message = new LobbyMessage(3, myAccount.accountId, output);
                lobbyMessageQueue.Add(message);

                foreach (int friendId in removeList)
                {
                    friends.Remove(friendId);
                }
            }
            if (friends.Count != 0)
            {
                string output2 = "";
                foreach (int friendId in friends)
                {
                    output2 += friendId + ",";
                    removeList.Add(friendId);
                }
                output2 = output2.Substring(0, output2.Length - 1);

                //send last message
                LobbyMessage message2 = new LobbyMessage(3, myAccount.accountId, output2);
                lobbyMessageQueue.Add(message2);

            }
        }

        public void FillPendingList(string data)
        {
            //string manipulation here, data = 1,degor|2,osuryn|...
            //make friends class with new 'pending'-constructor
            int index;
            int pendingId;
            string pendingName;
            string backup = data;

            guiController.ClearSocialLists(3);

            index = data.IndexOf(",");

            while (index != -1)
            {
                pendingId = Convert.ToInt32(data.Substring(0, index));
                data = data.Substring(index + 1);

                index = data.IndexOf("|");
                pendingName = data.Substring(0, index != -1 ? index : data.Length);
                data = data.Substring(index != -1 ? index + 1 : data.Length);

                index = data.IndexOf(",");

                pendingList.Add(new Friend(pendingId, pendingName, true));
            }
        }

        private void AcceptedPendingFriend(string data)
        {
            int index = data.IndexOf(",");
            int accepted = Convert.ToInt32(data.Substring(0, index));
            data = data.Substring(index + 1);
            string FriendId = data;

            if (accepted == 1)
            {
                Friend friend = pendingList.Find(f => f.friendId == Convert.ToInt32(FriendId));
                string friendName = friend.friendName;
                guiController.RemovePendingFriend(friendName);
                pendingList.Remove(friend);
            }
            else
            {
                guiController.ShowMessageBox("error occurred while accepting friend request");
            }
        }

        private void RejectedPendingFriend(string data)
        {
            int index = data.IndexOf(",");
            int accepted = Convert.ToInt32(data.Substring(0, index));
            data = data.Substring(index + 1);
            string FriendId = data;

            if (accepted == 1)
            {
                Friend friend = pendingList.Find(f => f.friendId == Convert.ToInt32(FriendId));
                string friendName = friend.friendName;
                guiController.RemovePendingFriend(friendName);
                pendingList.Remove(friend);
            }
            else
            {
                guiController.ShowMessageBox("Error occured while rejecting friend request");
            }
        }

        private void RemovedFriendship(string data)
        {
            int index = data.IndexOf(",");
            int accepted = Convert.ToInt32(data.Substring(0, index));
            data = data.Substring(index + 1);
            string FriendId = data;

            if (accepted == 1)
            {
                Friend friend = friendList.Find(f => f.friendId == Convert.ToInt32(FriendId));
                string friendName = friend.friendName;
                guiController.RemoveFriend(friendName);
                friendList.Remove(friend);
            }
            else
            {
                guiController.ShowMessageBox("error occurred while removing friendship");
            }
        }

        public int getFriendIdByName(string name)
        {
            foreach (Friend f in pendingList)
            {
                if (f.friendName == name)
                {
                    return f.friendId;
                }
            }

            foreach (Friend f in friendList)
            {
                if (f.friendName == name)
                {
                    return f.friendId;
                }
            }
            return 0;
        }

        #endregion

        #region Channels

        public ChatChannel GetChannelByName(string name)
        {
            ChatChannel channel = null;
            foreach (ChatChannel chatChannel in channelList)
            {
                if (chatChannel.channelName == name)
                {
                    channel = chatChannel;
                }
            }
            return channel;
        }

        public ChatChannel GetChannelById(int Id)
        {
            ChatChannel channel = null;
            foreach (ChatChannel chatChannel in channelList)
            {
                if (chatChannel.channelId == Id)
                {
                    channel = chatChannel;
                }
            }
            return channel;
        }

        private void GetChatchannels(string data)
        {
            int index = -1;
            string dataBackup = data;
            index = data.IndexOf(",");
            while (index > -1)
            {
                try
                {
                    string localBackup = dataBackup.Substring(0, index);
                    index = -1;
                    index = data.IndexOf("|");
                    if (index > -1)
                    {
                        int channelId = Convert.ToInt32(data.Substring(0, index));
                        data = data.Substring(index + 1);
                        index = -1;
                        index = data.IndexOf("|");
                        if (index > -1)
                        {
                            string channelName = data.Substring(0, index);
                            int ownerId = Convert.ToInt32(data.Substring(index + 1));
                            List<int> userList = new List<int>();
                            userList.Add(ownerId);
                            channelList.Add(new ChatChannel(false, channelId, channelName, ownerId, userList));
                            dataBackup = dataBackup.Substring(index + 1);
                        }
                        index = -1;
                        index = dataBackup.IndexOf(",");
                    }

                    index = -1;
                    index = dataBackup.IndexOf(",");
                }
                catch
                {
#if DEBUG
                    guiController.ShowMessageBox("Unexpected error while getting channellist");
#endif
                }
            }
            if (dataBackup.Length != 0)
            {
                string localBackup = dataBackup;
                index = -1;
                index = data.IndexOf("|");
                if (index > -1)
                {
                    int channelId = Convert.ToInt32(data.Substring(0, index));
                    data = data.Substring(index + 1);
                    index = -1;
                    index = data.IndexOf("|");
                    if (index > -1)
                    {
                        string channelName = data.Substring(0, index);
                        int ownerId = Convert.ToInt32(data.Substring(index + 1));
                        List<int> userList = new List<int>();
                        userList.Add(ownerId);
                        channelList.Add(new ChatChannel(false, channelId, channelName, ownerId, userList));
                        dataBackup = dataBackup.Substring(index + 1);
                    }
                }
            }
        }

        public void AddChatChannel(string data)
        {
            int index = -1;
            index = data.IndexOf("|");
            int channelId = Convert.ToInt32(data.Substring(0, index));
            if (channelId == -1)
            {
                guiController.AddToChat("Channel " + data.Substring(index+1) + " already exists.");
            }
            else
            {
                data = data.Substring(index + 1);
                index = -1;
                index = data.IndexOf("|");
                string channelName = data.Substring(0, index);
                int ownerId = Convert.ToInt32(data.Substring(index + 1));
                List<int> userList = new List<int>();
                userList.Add(ownerId);
                channelList.Add(new ChatChannel(true, channelId, channelName, ownerId, userList));
            }
        }

        private void AddChannelMessage(string output, string color)
        {
            int index = output.IndexOf("|");
            int channelId = Convert.ToInt32(output.Substring(0, index));
            string message = output.Substring(index + 1);

            ChatChannel channel = null;
            channel = GetChannelById(channelId);

            if (channel != null)
            {
                channel.reveivedText.Enqueue(guiController.GetColoredText(message, color));
            }
            else
            {
#if DEBUG
                guiController.ShowMessageBox("No channel with id: " + channelId + " found. The received message was: " + message);
#endif
            }
        }

        private void JoinChannel(string data)
        {
            int index = -1;
            string backup = data;

            guiController.UnityLog("joined channel with data: " + data);

            index = backup.LastIndexOf("|");
            int id = Convert.ToInt32(backup.Substring(0, index));

            ChatChannel channel = GetChannelById(id);

            if (channel != null)
            {
                channel.joined= true;
                backup = backup.Substring(0, index + 1);
                while (index != -1)
                {
                    index = -1;
                    index = backup.LastIndexOf(",");
                    if (index != -1)
                    {
                        channel.userList.Add(Convert.ToInt32(backup.Substring(0, index)));
                        backup = backup.Substring(0, index + 1);
                    }
                }
            }
        }

        #endregion

        #region Guilds

        private void GetGuildInfo(string data)
        {
            string backup = data;
            int index = -1;

            index = backup.IndexOf('|');
            int id = Convert.ToInt32(backup.Substring(0, index));
            backup = backup.Substring(index + 1);

            index = -1;
            index = backup.IndexOf('|');
            string name = backup.Substring(0, index);
            string tag = backup.Substring(index + 1);

            myGuild = new Guild(id, name, tag);
            //guiController.UpdateGuildTitle(DomainController.getInstance().myGuild.guildName);
            guildInfoReceived = true;
            AddLobbyMessageToQueue(35, id.ToString());
            guiController.SetDebugText("Guild Added!");
        }

        private void GetGuildMembers(string data)
        {
            string backup = data.Substring(1);
            int index = -1;
            Guild guild = null;

            try
            {
                index = backup.IndexOf(',');
                int guildID = Convert.ToInt32(backup.Substring(0, index));
                backup = backup.Substring(index + 1);

                if (guildID == myGuild.guildId)
                {
                    guild = myGuild;
                }
                else
                {
#if DEBUG
                    guiController.ShowMessageBox("Got info about a guild the player is not part of.");
#endif
                }

                index = -1;
                index = backup.IndexOf(']');
                int count = Convert.ToInt32(backup.Substring(0, index));
                backup = backup.Substring(index + 1);

                if (count == 0)
                {
                    guild.ClearMembers();
                }

                index = -1;
                index = backup.IndexOf('|');
                while (index != -1)
                {
                    string backup2 = backup.Substring(0, index);
                    int index2 = -1;
                    index2 = backup2.IndexOf(',');
                    int id = Convert.ToInt32(backup2.Substring(0, index2));
                    backup2 = backup2.Substring(index2 + 1);

                    index2 = -1;
                    index2 = backup2.IndexOf(',');
                    byte flags = Convert.ToByte(backup2.Substring(0, index2));
                    backup2 = backup2.Substring(index2 + 1);

                    index2 = -1;
                    index2 = backup2.IndexOf('|');
                    string screenName = backup2;

                    GuildMember member = new GuildMember(guild.guildId, id, flags, screenName);
                    if (member.accountId == myAccount.accountId)
                    {
                        myAccount.SetGuildFlags(flags);
                    }
                    guild.AddMember(member);

                    backup = backup.Substring(index + 1);
                    index = -1;
                    index = backup.IndexOf('|');
                }

                index = -1;
                index = backup.IndexOf(',');
                int id2 = Convert.ToInt32(backup.Substring(0, index));
                backup = backup.Substring(index + 1);

                index = -1;
                index = backup.IndexOf(',');
                byte flags2 = Convert.ToByte(backup.Substring(0, index));
                backup = backup.Substring(index + 1);

                index = -1;
                index = backup.IndexOf('|');
                string screenName2 = backup;

                GuildMember member2 = new GuildMember(guild.guildId, id2, flags2, screenName2);
                guild.AddMember(member2);

                guiController.SetDebugText("Guild data added!");
            }
            catch (Exception e)
            {
                guiController.SetDebugText(e.ToString());
                throw;
            }
        }

        public int GetGuildMemberIdByScreenName(string name)
        {
            int id = -1;
            foreach (GuildMember member in myGuild.userList)
            {
                if (member.screenName == name)
                {
                    id = member.accountId;
                }
            }
            return id;
        }

        #endregion

        #region Party

        private void FormatPartyMembers(string data)
        {
            string[] array;
            array = data.Split('|');

            myParty.userList.Clear();

            foreach (string account in array)
            {
                string[] array2;
                array2 = account.Split('|');
                Account toAdd = new Account(Convert.ToInt32(array2[0]), Convert.ToByte(array2[1]), array2[2], Convert.ToInt32(array2[3]));
                FullyKnownAccounts.Add(toAdd);
                myParty.adduser(toAdd.accountId);
            }

        }

        private void GetPartyInvite(string data)
        {
            int index = -1;
            index = data.LastIndexOf("|");
            int partyId = Convert.ToInt32(data.Substring(0, index));
            index = -1;
            data = data.Substring(index + 1);
            index = data.LastIndexOf("|");
            invitingPlayer = Convert.ToInt32(data.Substring(0, index));
            string inviterName = data.Substring(index + 1);

            invitingPlayer = 

            invitedParty = partyId;

            guiController.ShowQuestionBox(LocalizedStrings.str_partyRequest , "Player " + inviterName + " has invited you to join his party.", AcceptPartyInvite, DeclinePartyInvite);
        }

        public void AcceptPartyInvite()
        {
            AddLobbyMessageToQueue(42, invitingPlayer + "|" + invitedParty);
            myAccount.partyID = invitedParty;
            myParty = new Party(myAccount, myAccount.partyID);
        }

        public void DeclinePartyInvite()
        {
            AddLobbyMessageToQueue(43, invitingPlayer + "|" + invitedParty.ToString());
        }

        #endregion

        #region Extra Functions

        public void setMode(int mode, string ip = null)
        {
            switch (mode)
            {
                case 0:
                    if (ip != null)
                    {
                        loginServer = new Server(26800, "Loginserver", "Online", ip);
                    }
                    else
                    {
                        loginServer = new Server(26800, "Loginserver", "Online", "192.168.189.5");
                    }
                    break;
                case 1:
                    loginServer = new Server(26800, "Loginserver", "Online", "127.0.0.1");
                    break;
                default:
                    loginServer = persistenceController.getLoginServer();
                    break;
            }
        }

        private List<int> getIntListFromData(string data)
        {
            List<int> returnList = new List<int>();
            int index = -1;
            string dataBackup = data;
            index = data.IndexOf(",");
            while (index > -1)
            {
                try
                {
                    if (index > -1)
                    {
                        returnList.Add(Convert.ToInt32(dataBackup.Substring(0, index)));
                        dataBackup = dataBackup.Substring(index + 1);
                    }
                    index = -1;
                    index = dataBackup.IndexOf(",");
                }
                catch
                {


                }
            }
            if (dataBackup.Length != 0)
            {
                returnList.Add(Convert.ToInt32(dataBackup.Substring(0, dataBackup.Length)));
            }
            return returnList;
        }

        public void SendLogin(string username, string password)
        {
            string hash = GetMd5Hash(username.ToLower() + "ytrezaisbetterthanytrewq" + password);
            networkController.SendLogin(username, GetMd5Hash(GetMd5Hash(hash.Substring(16, 16) + "hasheniszomainstream" + hash.Substring(0, 16))));
        }

        public void SendLogout()
        {
            networkController.SendLogout();
        }

        private string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        public string PrepareMessageToSend(Message message)
        {
            string data = "";

            if (message is ChatMessage)
            {
                ChatMessage chatmessage = (ChatMessage)message;
                data = "[" + chatmessage.type + "," + chatmessage.recieverID + "," + chatmessage.senderID + "]" + chatmessage.message;
            }
            if (message is LobbyMessage)
            {
                data = "[" + message.type + "," + DomainController.getInstance().myAccount.accountId + "]" + message.message;
            }
            return data;
        }

        public void ReceiveAccountInfo(string accountInfo, bool myAccount)
        {
            int userId;
            byte flags;
            int index = -1;
            string screenName;
            int guildId = -1;

            index = accountInfo.IndexOf("@");
            string ipData = accountInfo.Substring(index);
            string outputbackup = accountInfo.Substring(0, index);

            index = outputbackup.IndexOf(",");
            userId = Convert.ToInt32(outputbackup.Substring(0, index));

            outputbackup = outputbackup.Substring(index + 1);
            index = -1;
            index = outputbackup.IndexOf(",");
            flags = Convert.ToByte(Convert.ToInt32(outputbackup.Substring(0, index)));

            outputbackup = outputbackup.Substring(index + 1);
            index = -1;
            index = outputbackup.IndexOf(",");
            screenName = outputbackup.Substring(0, index);

            guildId = Convert.ToInt32(outputbackup.Substring(index + 1));

            Account account = new Account(userId, flags, screenName, guildId);

            if (myAccount)
            {
                this.myAccount = account;
                setServerIpAdresses(ipData);
            }
            else
            {
                FullyKnownAccounts.Add(account);
            }
        }

        public void setServerIpAdresses(string ipData)
        {
            string chtExt = "";
            int chtPort = -1;
            string chtInt = "";
            string lbyExt = "";
            int lbyPort = -1;
            string lbyInt = "";
            int index = -1;
            string data = ipData;

            index = data.IndexOf("@");
            if (index != -1)
            {
                data = data.Substring(index + 1);
            }

            index = data.IndexOf(":");
            chtExt = data.Substring(0, index);
            data = data.Substring(index + 1);

            index = data.IndexOf("#");
            chtPort = Convert.ToInt32(data.Substring(0, index));
            data = data.Substring(index + 1);

            index = data.IndexOf("|");
            chtInt = data.Substring(0, index);
            data = data.Substring(index + 1);

            index = data.IndexOf(":");
            lbyExt = data.Substring(0, index);
            data = data.Substring(index + 1);

            index = data.IndexOf("#");
            lbyPort = Convert.ToInt32(data.Substring(0, index));
            data = data.Substring(index + 1);

            index = data.IndexOf("|");
            lbyInt = data.Substring(0, index);

            networkController.chatPort = chtPort;
            networkController.lobbyPort = lbyPort;

            string ownExt = "";
            try
            {
                string url = "http://checkip.dyndns.org";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                ownExt = a3[0];

                if (ownExt == "")
                {
                    string whatIsMyIp = "http://whatismyip.com";
                    string getIpRegex = @"(?<=<TITLE>.*)\d*\.\d*\.\d*\.\d*(?=</TITLE>)";
                    WebClient wc = new WebClient();
                    UTF8Encoding utf8 = new UTF8Encoding();
                    string requestHtml = "";
                    try
                    {
                        requestHtml = utf8.GetString(wc.DownloadData(whatIsMyIp));
                    }
                    catch (WebException we)
                    {
                        // do something with exception
                        Console.Write(we.ToString());
                    }
                    Regex r = new Regex(getIpRegex);
                    Match m = r.Match(requestHtml);
                    ownExt = "";
                    if (m.Success)
                    {
                        ownExt = m.Value;
                    }
                }
            }
            catch (Exception)
            {
                try
                {

                    string whatIsMyIp = "http://whatismyip.com";
                    string getIpRegex = @"(?<=<TITLE>.*)\d*\.\d*\.\d*\.\d*(?=</TITLE>)";
                    WebClient wc = new WebClient();
                    UTF8Encoding utf8 = new UTF8Encoding();
                    string requestHtml = "";
                    try
                    {
                        requestHtml = utf8.GetString(wc.DownloadData(whatIsMyIp));
                    }
                    catch (WebException we)
                    {
                        // do something with exception
                        Console.Write(we.ToString());
                    }
                    Regex r = new Regex(getIpRegex);
                    Match m = r.Match(requestHtml);
                    ownExt = "";
                    if (m.Success)
                    {
                        ownExt = m.Value;
                    }
                }
                catch (Exception)
                {
                    Console.Write("U ARE NOT CONNECTED TO THE INTERWEBS");
                }
            }

            index = -1;
            index = ownExt.LastIndexOf('.');
            string ownExtIp = ownExt.Substring(0, index);

            IPHostEntry host;
            string ownInt = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ownInt = ip.ToString();
                    break;
                }
            }

            if (!chtExt.Contains(ownExtIp))
            {
                networkController.chatServerIpAddress = chtExt;
                Console.Write("the correct IP for Chat would be chtExt: " + chtExt);
            }
            else
            {
                if (ownInt == chtInt)
                {
                    networkController.chatServerIpAddress = "127.0.0.1";
                    Console.Write("the correct IP for Chat would be 127.0.0.1 ");
                }
                else
                {
                    networkController.chatServerIpAddress = chtInt;
                    Console.Write("the correct IP for Chat would be chtInt: " + chtInt);
                }

            }

            if (!lbyExt.Contains(ownExtIp))
            {
                networkController.lobbyServerIpAddress = lbyExt;
                Console.Write("the correct IP for Lobby would be lbyExt: " + lbyExt);
            }
            else
            {
                if (ownInt == lbyInt)
                {
                    networkController.lobbyServerIpAddress = "127.0.0.1";
                    Console.Write("the correct IP for Lobby would be 127.0.0.1 ");
                }
                else
                {
                    networkController.lobbyServerIpAddress = lbyInt;
                    Console.Write("the correct IP for Lobby would be lbyInt: " + lbyInt);
                }
            }
        }

        #endregion

        #region Commands

        public void Command(string input, bool fromChat)
        {
            int index = -1;
            string backup = input;

            Queue<string> parameters = new Queue<string>();
            
            do
            {
                index = backup.LastIndexOf(' ');
                if (index != -1)
                {
                    parameters.Enqueue(backup.Substring(0, index));
                    backup = backup.Substring(index+1);
                }

            } while (index != -1);

            parameters.Enqueue(backup);

           string testParm =  parameters.Dequeue();

           switch (testParm.ToLower())
            {
               case "help":
                    guiController.AddToChat("commands: join, createchannel, invite, fps, language");
                    break;
                case "join":
                    if (parameters.Count >= 1)
                    {
                        string channelname = "";
                        while (parameters.Count > 0)
                        {
                            channelname += parameters.Dequeue();
                        }

                        ChatChannel channel = DomainController.getInstance().GetChannelByName(channelname);
                        if (channel != null)
                        {
                                DomainController.getInstance().AddLobbyMessageToQueue(21, channel.channelId.ToString());//join channel
                                //DomainController.getInstance().AddLobbyMessageToQueue(23, channel.channelId.ToString());//request channel details
                                //guiController.activeChannel = channel.channelId;
                            }
                            else
                            {

                                guiController.AddToChat("No channel found with name: " + channelname);
                            }
                        }
                    else
                    {
                        guiController.AddToChat("Invalid join command, usage: /join [channelname]");
                    }
                    break;
               case "createchannel":
                    if (parameters.Count == 1)
                    {
                        string channelname2 = parameters.Dequeue();
                        domainController.AddLobbyMessageToQueue(1, channelname2);
                    }
                    else
                    {
                        guiController.AddToChat("Invalid createchannel command, usage: /createchannel [channelname]");
                    }
                    break;
               case "invite":
                    if (parameters.Count == 1)
                    {
                        string playerName = parameters.Dequeue();
                        domainController.AddLobbyMessageToQueue(40, myAccount.partyID + "|" + playerName);
                    }
                    else
                    {
                        guiController.AddToChat("Invalid invite command, usage: /invite [playername]");
                    }
                    break;
               case "fps":
                    if (guiController.fpsCounter)
                    {
                        guiController.fpsCounter = false;
                        guiController.AddToChat("FPS counter disabled");
                    }
                    else
                    {
                        guiController.fpsCounter = true;
                        guiController.AddToChat("FPS counter enabled");
                    }
                    break;
               case "language":
                    if (parameters.Count == 1)
                    {
                        string language = parameters.Dequeue();
                        if (LocalizedStrings.SetLanguage(language))
                        {
                            guiController.AddToChat("Switching language to " + language.ToUpper());
                        }
                        else
                        {
                            guiController.AddToChat("No language " + language.ToUpper() + " found defaulting to English");
                        }
                    }
                    else
                    {
                        guiController.AddToChat("Invalid language command, usage: /language [abriviated language (EN, NL)]");
                    }
                    break;
                default:
                    guiController.AddToChat("/" + testParm + " is not a recognized command.");
                    break;
            }
            //guiController.SetDebugText("Command: " + input);
        }
        #endregion

        #region Threads

        private void startThreads()
        {
            if (ClientOn)
            {
                //lobby messages
                LobbyMessageHandler = new Thread(new ThreadStart(LobbyMessageHandlerThread));
                LobbyMessageHandler.Name = "Lobby Message Handler Thread";
                LobbyMessageHandler.IsBackground = true;
                LobbyMessageHandler.Start();

                //Chat messages
                ChatMessageHandler = new Thread(new ThreadStart(ChatMessageHandlerThread));
                ChatMessageHandler.Name = "Chat Message Handler Thread";
                ChatMessageHandler.IsBackground = true;
                ChatMessageHandler.Start();
            }
        }
		
		private void ChatMessageHandlerThread()
        {
            while (ClientOn)
            {
                if (IncommingChatMessage.Count > 0)
                {
                    this.GetMessage("chatlistener", IncommingChatMessage.Dequeue());
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        private void LobbyMessageHandlerThread()
        {
            while (ClientOn)
            {
                if (IncommingLobbyMessage.Count > 0)
                {
                    this.GetMessage("lobbylistener", IncommingLobbyMessage.Dequeue());
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        #endregion
    }
}
