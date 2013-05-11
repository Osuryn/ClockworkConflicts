using UnityEngine;
using System.Collections.Generic;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class Window : Control
    {
        public GUI.WindowFunction windowFunction;
        public int id { get; set; }
        public List<Control> children { get; set; }
        public Rect windowRect { get; set; }
        public Rect savedScreen { get; set; }
        public Rect unscaledRect { get; set; }

        public Window(string name) : base(name)
        {
            children = new List<Control>();
            id = windowCount;
            windowCount++;
            savedScreen = guiController.screenRect;
        }

        public override void Render()
        {         
            if (visible)
            {
                int fontsize = GUI.skin.window.fontSize;
                if (savedScreen != guiController.screenRect)
                {
                    float x = (windowRect.x / savedScreen.width) * GuiController.optimalWidth;
                    float y = (windowRect.y / savedScreen.height) * GuiController.optimalHeight;
                    savedScreen = guiController.screenRect;
                    windowRect = guiController.ScaledRect(new Rect(x, y, unscaledRect.width, unscaledRect.height), parentSurface);
                }
                GUI.skin.window.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);
                windowRect = GUI.Window(id, windowRect, windowFunction, text);
                GUI.skin.window.fontSize = fontsize;
            }
        }

        public void RenderChildren()
        {
            if (visible)
            {
                foreach (Control child in children)
                {
                    child.Render();
                }
            }
        }

        public override void SetRect(Rect rect)
        {
            location = new Point(rect.x, rect.y);
            size = new Size(rect.width, rect.height);
            unscaledRect = rect;
            windowRect = guiController.ScaledRect(unscaledRect, parentSurface);
            savedScreen = guiController.screenRect;
        }
    }
}