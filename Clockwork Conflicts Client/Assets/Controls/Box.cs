using UnityEngine;

using System.Collections.Generic;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class Box : Control
    {

        private List<Control> children { get; set; }

        public Box(string name) : base(name)
        {
            children = new List<Control>();
        }

        public override void Render()
        {
            if (visible)
            {
                GUI.skin.box.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);
                GUI.SetNextControlName(name);
                GUI.Box(guiController.ScaledRect(this.GetRect(), parentSurface), text);
                RenderChildren();
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

        public void AddChild(Control child)
        {
            child.parentSurface = this.GetRect();
            children.Add(child);
        }
    }
}