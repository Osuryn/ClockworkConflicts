using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMTD_Client.Domain
{
    public class Notification
    {

        static int notificationCount = 0;

        public int type { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int senderId { get; set; }
        public string senderName { get; set; }

        //Types
        //===================
        //  0   =   just text
        //  1   =   party invite
        //  2   =   Guild invite

        public Notification(int type, string title, string content, int senderId = -1, string senderName = null)
        {
            this.id = notificationCount;
            notificationCount++;
            this.type = type;
            this.title = title;
            this.content = content;
            this.senderId = senderId;
            this.senderName = senderName;
        }
    }
}
