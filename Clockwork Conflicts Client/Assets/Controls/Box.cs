using UnityEngine;
using System.Collections;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class Box : Control
    {

        public Box(string name) : base(name)
        { 
        }

        public override void Render()
        {
            GUI.skin.box.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);           
            GUI.SetNextControlName(name);
            GUI.Box(guiController.ScaledRect(this.GetRect(), parentSurface), text);         
        }
    }
}