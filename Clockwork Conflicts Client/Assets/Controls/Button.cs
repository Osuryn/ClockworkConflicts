using UnityEngine;

using System;
using System.Collections;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class Button : Control
    {

        public Button(string name) : base(name)
        { 
        }

        public override void Render()
        {
            GUI.skin.button.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);
            GUI.skin.button.fixedHeight = (int)Mathf.Ceil(GUI.skin.button.fontSize * 2f);
            Rect rect = new Rect(location.x, location.y, size.width, GUI.skin.button.fontSize * 2f);
            if (GUI.Button(guiController.ScaledRect(rect, parentSurface), text))
            {
                OnClicked(EventArgs.Empty);
            }         
        }

    }
}