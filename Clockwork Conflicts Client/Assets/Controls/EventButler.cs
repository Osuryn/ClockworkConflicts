using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace MMTD_Client.Controls
{
    class EventButler
    {

        public const int waitTime = 100;

        public int id { get; set; }
        public string name { get; set; }
        public bool handled { get; set; }
        private Timer timer;

        public EventButler(string name)
        {
            this.name = name;
            handled = false;

            timer = new Timer(waitTime);
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        private void Timer_Elapsed(object Sender, ElapsedEventArgs e)
        {
            handled = true;
            timer.Enabled = false;
        }
    }
}
