using UnityEngine;

using System;
using System.Collections;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class Button : Control
    {

        public Texture2D icon { get; set; }

        public Button(string name) : base(name)
        {
            icon = null;
        }

        public override void Render()
        {
            if (visible)
            {
                if (icon == null)
                {
                    GUI.skin.button.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);
                    //GUI.skin.button.fixedHeight = (int)Mathf.Ceil(GUI.skin.button.fontSize * 2f);
                    Rect rect = new Rect(location.x, location.y, size.width, GUI.skin.button.fontSize * 2f);
                    if (GUI.Button(guiController.ScaledRect(rect, parentSurface), text))
                    {
                        OnClicked(EventArgs.Empty);
                    }
                }
                else
                {
                    Rect rect = new Rect(location.x, location.y, size.width, size.height);//icon.height + GUI.skin.button.padding.top + GUI.skin.button.padding.bottom
                    if (GUI.Button(guiController.ScaledRect(rect, parentSurface), icon))
                    {
                        OnClicked(EventArgs.Empty);
                    }
                }
            }
        }

    }
}