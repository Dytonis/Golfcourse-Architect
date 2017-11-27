using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GA.Objects;

namespace GA.UI.Windows.PreviewMenu
{
    public class PreviewMenu : UIMenu
    {
        public Image imageForTinting;
        public Text GridTextPrefab;
        public TextDouble Flare;
        public Text title;
        public Text description;
        public GridLayoutGroup GridEffects;
        public Text price;
        public RawImage renderTexture;

        public void DrawFlare(string text, string subtext, Color color, Color tint)
        {
            if(Flare)
            {
                TextDouble textd = Instantiate(Flare, this.transform);
                textd.text = text;
                textd.Subtext.text = subtext;
                textd.FillText.color = color;
                textd.Subtext.FillText.color = color;
                imageForTinting.color = tint;
            }
        }
    }
}
