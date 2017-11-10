using GA.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GA.UI.Windows.HoleInfoMenu
{
    public class HoleInfoMenu : UIMenu
    {
        private Hole _h;
        public Hole AppliedHole
        {
            get
            {
                return _h;
            }
        }

        public Color ActiveTextColor;
        public Color UnactiveTextColor;

        public UITab[] Tabs = new UITab[4];

        public void Start()
        {
            foreach(UITab t in Tabs)
            {
                t.OnElementLoad();
            }
        }

        public void SetHole(Hole h)
        {
            _h = h;
        }

        public void TabClick(int id)
        {
            Tabs[id].transform.SetAsLastSibling();

            Tabs.ToList().ForEach(x =>
            {
                if (x != null)
                {
                    if (x.Page != null)
                    {
                        x.Page.gameObject.SetActive(false);
                        x.Page.OnMenuHide();
                    }
                    if (x.Label != null)
                        x.Label.color = UnactiveTextColor;
                }
            });

            if (Tabs[id].Page != null)
            {
                Tabs[id].Page.gameObject.SetActive(true);
                Tabs[id].Page.OnMenuShow();
            }
            if (Tabs[id].Label != null)
                Tabs[id].Label.color = ActiveTextColor;
        }
    }
}
