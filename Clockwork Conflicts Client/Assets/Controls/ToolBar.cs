using UnityEngine;

using System.Collections.Generic;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class Toolbar : Control
    {

        public int selectedIndex { get; set; }
        public List<string> items { get; set; }

        public Toolbar(string name) : base(name)
        {
            items = new List<string>();
            selectedIndex = -1;
        }

        public override void Render()
        {
            GUI.skin.button.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);           
            GUI.SetNextControlName(name);
            selectedIndex = GUI.Toolbar(guiController.ScaledRect(this.GetRect(), parentSurface), selectedIndex, items.ToArray());         
        }
    }
}