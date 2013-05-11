using UnityEngine;
using System.Collections;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class TextField : Control
    {

        public int maxLength { get; set; }
        public bool passwordField { get; set; }
        public char maskChar { get; set; }

        public TextField(string name) : base(name)
        {
            maxLength = 255;
            maskChar = '•';
        }

        public override void Render()
        {
            if (visible)
            {
                GUI.skin.textField.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);
                GUI.skin.textField.fixedHeight = (int)Mathf.Ceil(GUI.skin.textField.fontSize + GUI.skin.textField.border.top + GUI.skin.textField.border.bottom + GUI.skin.textField.padding.top + GUI.skin.textField.padding.bottom);
                GUI.skin.textField.fixedWidth = size.width * guiController.scale.x;
                Rect rect = guiController.ScaledRect(new Rect(location.x, location.y, size.width, GUI.skin.textField.fontSize * 2f), parentSurface);
                GUI.SetNextControlName(name);
                if (passwordField)
                {
                    text = GUI.PasswordField(rect, text, maskChar, maxLength);
                }
                else
                {
                    GUI.SetNextControlName(name);
                    text = GUI.TextField(rect, text, maxLength);
                }

                Event e = Event.current;

                if (GUI.GetNameOfFocusedControl() == name)
                {
                    //Debug.Log("pressed: " + e.keyCode);
                }
            }
        }
    }
}