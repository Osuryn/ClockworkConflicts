using UnityEngine;

using System.Linq;
using System.Collections.Generic;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class ScrollView : Control
    {
        public Vector2 scrollPosition { get; set; }
        public List<Control> children { get; set; }
        public Rect totalArea { get; set; }
        public bool autoScroll { get; set; }

        public ScrollView(string name) : base(name)
        {
            scrollPosition = Vector2.zero;
            children = new List<Control>();
            autoScroll = false;
        }

        public override void Render()
        {
            if (visible)
            {
                float tempWidth = 0;
                float tempHeight = 0;
                float lastHeight = totalArea.height;

                totalArea = new Rect();
                foreach (Control control in children)
                {
                    tempHeight += totalArea.height + control.size.height;
                    tempWidth += totalArea.width + control.size.width;
                }
                tempHeight += totalArea.height + GUI.skin.scrollView.padding.left + GUI.skin.scrollView.padding.right;
                tempWidth += totalArea.width + GUI.skin.scrollView.padding.bottom + GUI.skin.scrollView.padding.top;
                totalArea = guiController.ScaledRect(new Rect(totalArea.x, totalArea.y, tempWidth, tempHeight), parentSurface);

                if (lastHeight != totalArea.height)
                {
                    scrollPosition = new Vector2(0, totalArea.height);
                }

                scrollPosition = GUI.BeginScrollView(guiController.ScaledRect(GetRect(), parentSurface), scrollPosition, totalArea);
                foreach (Control control in children)
                {
                    control.Render();
                }
                GUI.EndScrollView();
            }
        }
    }
}