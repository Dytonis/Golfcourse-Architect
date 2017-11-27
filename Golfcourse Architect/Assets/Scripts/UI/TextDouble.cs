using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GA.UI
{
    public class TextDouble : MonoBehaviour
    {
        public Text FillText;
        public Text OutlineText;
        public TextDouble Subtext;

        public string text
        {
            get
            {
                return FillText.text;
            }
            set
            {
                FillText.text = value;
                OutlineText.text = value;
            }
        }

        public void updateText(string text)
        {
            this.text = text;
        }
    }

    [UnityEditor.CustomEditor(typeof(TextDouble))]
    [CanEditMultipleObjects]
    public class TextDoubleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TextDouble d = (TextDouble)target;
            if (d.FillText)
            {
                string text = GUILayout.TextArea(d.FillText.text, GUILayout.Height(40f));
                d.updateText(text);
            }
            else
            {
                GUILayout.TextArea("You must at least define a FillText.", GUILayout.Height(40f));
            }
            //a bit hacky, but the only way i know how to do this:
            if (d.FillText)
            {
                d.FillText.enabled = false;
                d.FillText.enabled = true;
            }
            if (d.OutlineText)
            {
                d.OutlineText.enabled = false;
                d.OutlineText.enabled = true;
            }
        }
    }
}
