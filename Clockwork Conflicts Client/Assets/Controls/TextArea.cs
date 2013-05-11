using UnityEngine;
using System.Collections.Generic;

using MMTD_Client.Gui;

namespace MMTD_Client.Controls
{
    public class TextArea : Control
    {

        public int maxLines { get; set; }
        public Queue<string> lines { get; set; }

        public TextArea(string name) : base(name)
        {
            lines = new Queue<string>();
            maxLines = 100;
        }

        public override void Render()
        {
            if (visible)
            {
                while (lines.Count > maxLines)
                {
                    lines.Dequeue();
                }
                string output = "";

                foreach (string line in lines)
                {
                    //if (line != lines.Peek())
                    //{
                    output += "\n";
                    //}
                    output += line;
                }
                text = output;

                GUI.skin.textArea.richText = true;
                GUI.skin.textArea.fontSize = (int)Mathf.Ceil(fontSize * guiController.scale.y);
                var boxHeight = GUI.skin.textArea.CalcHeight(new GUIContent(text), size.width * guiController.scale.x);
                float lineCount = boxHeight / fontSize;
                size.height = boxHeight + (lineCount * 3.333f) + GUI.skin.textArea.border.top + GUI.skin.textArea.border.bottom + GUI.skin.textArea.padding.top + GUI.skin.textArea.padding.bottom * guiController.scale.y; //(fontSize/3.6f)
                //GUI.skin.textArea.fixedWidth = size.width * guiController.scale.x;
                Rect rect2 = guiController.ScaledRect(new Rect(location.x, location.y, size.width, size.height), parentSurface);
                //rectToFill = rect2;
                text = GUI.TextArea(rect2, text);
            }
        }
    }
}