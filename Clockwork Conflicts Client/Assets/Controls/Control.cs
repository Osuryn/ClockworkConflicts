using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class Control
    {

        public static int windowCount;
        public static Control selected;

        public string name { get; set; }
        public string text { get; set; }
        public Point location { get; set; }
        public Size size { get; set; }
        public GuiController guiController { get; private set; }
        public int fontSize { get; set; }
        public Rect parentSurface { get; set; }
        public Color color { get; set; }
        public bool autoSize { get; set; }
        public bool disabled { get; set; }
        public bool visible { get; set; }

        private List<EventButler> eventsToComplete = new List<EventButler>();

        public event EventHandler Clicked;

        public Control(string name)
        {
            this.name = name;
            guiController = GuiController.getInstance();
            //guiController.SetDebugText("Control created with name: " + this.name);

            //default values
            autoSize = true;
            fontSize = 12;
            color = Color.white;
            parentSurface = guiController.screenRect;
            location = new Point(0, 0);
            text = "";
            visible = true;
            disabled = false;
            //location.x = 0f;
            //location.y = 0f;
        }

        public virtual void SetRect(Rect rect)
        {
            location = new Point(rect.x, rect.y);
            size = new Size(rect.width, rect.height);
        }

        public Rect GetRect()
        {
            return new Rect(location.x, location.y, size.width, size.height);
        }

        public virtual void Render()
        {
            //nothing happens
        }

        public virtual void OnClicked(EventArgs e)
        {
            if (Clicked != null)
                Clicked(this, e);
        }

        public bool EventCompleted(string name)
        {
            bool foundComplete = false;
            EventButler foundButler = null;

            foreach (EventButler butler in eventsToComplete)
            {
                if (butler.name == name)
                {
                    foundButler = butler;
                    if (butler.handled)
                    {
                        foundComplete = true;
                    }
                }
            }

            if (foundButler == null)
            {
                eventsToComplete.Add(new EventButler(name));
                return false;
            }

            if (foundComplete)
            {
                eventsToComplete.Remove(foundButler);
                return true;
            }

            return false;
        }
    }
}
