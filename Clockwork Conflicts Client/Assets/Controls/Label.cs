using UnityEngine;
using System.Collections;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class Label : Control
    {

        public Label(string name) : base(name)
        { 
        }

        public override void Render()
        {
            if (visible)
            {
                GUIStyle style = new GUIStyle();
                var textSize = GUI.skin.label.CalcSize(new GUIContent(text));
                style.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);
                style.normal.textColor = color;
                Rect rect = guiController.ScaledRect(new Rect(location.x, location.y, textSize.x, textSize.y), parentSurface);
                GUI.SetNextControlName(name);
                GUI.Label(rect, text, style);
            }
        }
    }
}